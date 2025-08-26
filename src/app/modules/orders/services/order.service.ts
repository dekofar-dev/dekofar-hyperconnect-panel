// src/app/modules/orders/services/order.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable, forkJoin, map, switchMap, of } from 'rxjs';
import { OrderModel } from '../models/order.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /**
   * Shopify ve manuel siparişleri birleştirip tarih sırasına göre döner (sayfalı Shopify).
   */
  getCombinedOrders(
    limit: number,
    pageInfo?: string,
    direction: 'next' | 'prev' = 'next'
  ): Observable<{ items: OrderModel[]; nextPageInfo?: string; previousPageInfo?: string }> {
    const shopify$ = this.http.get<{ items: any[]; nextPageInfo?: string | null }>(
      `${this.apiUrl}/Shopify/orders-paged?limit=${limit}${
        pageInfo ? `&pageInfo=${encodeURIComponent(pageInfo)}` : ''
      }`
    );

    const manual$ = this.getManualOrders();

    return forkJoin([shopify$, manual$]).pipe(
      map(([shopifyRes, manualOrders]) => {
        const shopifyOrders: OrderModel[] = (shopifyRes.items || []).map((o: any) => this.mapShopifyOrder(o));
        const formattedManualOrders: OrderModel[] = (manualOrders || []).map(order => ({
          ...order,
          source: 'Manual' as const,
          currency: 'TRY',
          createdByAvatarUrl: order.createdByAvatarUrl ?? undefined
        }));

        const combined: OrderModel[] = [...formattedManualOrders, ...shopifyOrders].sort(
          (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );

        return {
          items: combined,
          nextPageInfo: shopifyRes.nextPageInfo ?? undefined,
          previousPageInfo: direction === 'prev' ? pageInfo : undefined
        };
      })
    );
  }

  /**
   * (Opsiyonel) Front-end’de detay toplama: arama → ID listesi → tekil detail çağrıları.
   * Büyük listelerde rate-limit ihtimali olduğundan max (default 50) ile sınırlar.
   */
  searchOrdersDetailedFrontOnly(query: string, max = 50): Observable<OrderModel[]> {
    const url = `${this.apiUrl}/Shopify/orders/search?query=${encodeURIComponent(query)}`;

    return this.http.get<any[]>(url).pipe(
      map(list => (list || []).map((o: any) => o.id)),                 // 1) id’leri al
      map(ids => Array.from(new Set(ids)).slice(0, max)),              // 2) tekilleştir + sınırla
      switchMap(ids => {
        if (!ids.length) return of<OrderModel[]>([]);
        const calls = ids.map((id: any) => this.getShopifyOrderById(String(id))); // 3) detail çağrıları
        return forkJoin(calls);
      }),
      map(items =>
        items.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      )
    );
  }

  /**
   * Manuel + Shopify araması (genel).
   */
  searchOrders(query: string): Observable<OrderModel[]> {
    const manual$ = this.http
      .get<any[]>(`${this.apiUrl}/manual-orders/search?query=${encodeURIComponent(query)}`)
      .pipe(map(results => (results || []).map(order => this.mapManualOrder(order))));

    const shopify$ = this.http
      .get<any[]>(`${this.apiUrl}/Shopify/orders/search?query=${encodeURIComponent(query)}`)
      .pipe(map(results => (results || []).map(order => this.mapShopifyOrder(order))));

    return forkJoin([manual$, shopify$]).pipe(
      map(([manualOrders, shopifyOrders]) =>
        [...manualOrders, ...shopifyOrders].sort(
          (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        )
      )
    );
  }

  /**
   * (Kısayol) Sadece Shopify araması — Excel/export senaryoları için idealdir.
   */
  searchOrdersShopifyOnly(query: string): Observable<OrderModel[]> {
    return this.http
      .get<any[]>(`${this.apiUrl}/Shopify/orders/search?query=${encodeURIComponent(query)}`)
      .pipe(
        map(results => (results || []).map(order => this.mapShopifyOrder(order))),
        map(list => list.sort((a, b) => +new Date(b.createdAt) - +new Date(a.createdAt)))
      );
  }

  /**
   * Tüm siparişleri al (sayfalama yok).
   */
  getAllOrders(): Observable<OrderModel[]> {
    return forkJoin([this.getManualOrders(), this.getShopifyOrders()]).pipe(
      map(([manualOrders, shopifyOrders]) =>
        [...manualOrders, ...shopifyOrders].sort(
          (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        )
      )
    );
  }

  getManualOrders(): Observable<OrderModel[]> {
    return this.http.get<any[]>(`${this.apiUrl}/manual-orders`).pipe(
      map(response => (response || []).map(order => this.mapManualOrder(order)))
    );
  }

  getShopifyOrders(): Observable<OrderModel[]> {
    const url = `${this.apiUrl}/Shopify/orders-paged?limit=100`;
    return this.http.get<{ items: any[] }>(url).pipe(
      map(response => (response.items || []).map(order => this.mapShopifyOrder(order)))
    );
  }

  getManualOrderById(id: string): Observable<OrderModel> {
    return this.http.get<any>(`${this.apiUrl}/manual-orders/${id}`).pipe(
      map(data => this.mapManualOrder(data))
    );
  }

  /**
   * DÜZELTİLDİ: Tekil Shopify sipariş endpoint’i → /api/Shopify/orders/{id}
   */
  getShopifyOrderById(id: string): Observable<OrderModel> {
    return this.http.get<any>(`${this.apiUrl}/Shopify/orders/${id}`).pipe(
      map(data => this.mapShopifyOrder(data))
    );
  }

  createManualOrder(order: {
    customerName: string;
    customerSurname?: string;
    phone?: string;
    email?: string;
    address?: string;
    city?: string;
    district?: string;
    paymentType?: string;
    orderNote?: string;
    discountId?: string;
    discountName?: string;
    discountType?: string;
    discountValue?: number;
    items: {
      productId: string;
      productName: string;
      variantId?: string;
      variantName?: string;
      quantity: number;
      price: number;
    }[];
  }): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/manual-orders`, order);
  }

  /**
   * Manuel siparişi OrderModel’e mapler.
   */
  private mapManualOrder(order: any): OrderModel {
    return {
      id: String(order.id),
      orderNumber: order.orderNumber ?? String(order.id),
      source: 'Manual',
      createdAt: order.createdAt,
      status: order.status,
      customerName: order.customerName,
      customerSurname: order.customerSurname,
      phone: order.phone,
      email: order.email,
      address: order.address,
      city: order.city,
      district: order.district,
      paymentType: order.paymentType,
      orderNote: order.orderNote,
      discountId: order.discountId,
      discountName: order.discountName,
      discountType: order.discountType,
      discountValue: order.discountValue,
      totalAmount: order.totalAmount,
      bonusAmount: order.bonusAmount,
      currency: 'TRY',
      createdByAvatarUrl: order.createdByAvatarUrl ?? undefined,
      items: (order.items || []).map((item: any) => ({
        productId: item.productId,
        productName: item.productName,
        quantity: item.quantity,
        price: item.price,
        variantId: item.variantId,
        variantName: item.variantName
      }))
    };
  }

  /**
   * Shopify siparişini OrderModel’e mapler.
   * - Adres/İl/İlçe için shipping yoksa billing ve note_attributes fallback’ları.
   * - TR düzeni: province → İL, city → İLÇE (ama notlar öncelikli).
   */
private mapShopifyOrder(order: any): OrderModel {
  const financial = (order.financial_status || '').toLowerCase();
  const fulfillment = (order.fulfillment_status || '').toLowerCase();

  let status: string;
  if (financial === 'paid' && fulfillment === 'fulfilled') status = 'Tamamlandı';
  else if (financial === 'paid' && fulfillment === 'unfulfilled') status = 'Hazırlanıyor';
  else if (financial === 'refunded') status = 'İade Edildi';
  else if (financial === 'partially_refunded') status = 'Kısmi İade';
  else if (financial === 'pending') status = 'Beklemede';
  else if (financial === 'authorized') status = 'Onaylandı';
  else if (financial === 'voided') status = 'İptal';
  else status = 'Bilinmiyor';

  // Adres: shipping varsa onu, yoksa billing
  const addr = order.shipping_address ?? order.billing_address ?? {};
  const address = [addr.address1, addr.address2].filter(Boolean).join(' ').trim();

  // Notlardan Şehir/İlçe okunabiliyorsa onları tercih et
  const noteIl   = this.getNoteAttr(order, ['şehir', 'sehir', 'il']);
  const noteIlce = this.getNoteAttr(order, ['ilçe', 'ilce']);

  // Shopify alanları: TR için çoğu mağazada city=İL, province=İLÇE kullanılıyor
  // Elindeki örnekte de (Ankara / Polatlı) bu şekilde.
  const city    = (noteIl   ?? addr.city     ?? '').toString().trim();     // İL
  const district= (noteIlce ?? addr.province ?? '').toString().trim();     // İLÇE

  // Telefonu en iyi kaynaktan çek ve normalize et (10 hane, 0'sız)
  const phone = this.pickBestPhone(order);

  const email = order.email || order.contact_email || order.customer?.email || '';

  return {
    id: order.id?.toString() ?? '',
    orderNumber: order.order_number ?? order.name ?? order.id?.toString(),
    source: 'Shopify',
    createdAt: order.created_at,
    status,

    customerName: `${order.customer?.first_name ?? addr.first_name ?? ''} ${order.customer?.last_name ?? addr.last_name ?? ''}`
      .trim() || 'Shopify Müşterisi',
    phone,
    email,

    address,
    city,       // İL
    district,   // İLÇE

    paymentType: 'Kredi Kartı',
    orderNote: order.note,
    totalAmount: parseFloat(order.total_price ?? '0') || 0,
    currency: order.currency ?? 'TRY',
    createdByAvatarUrl: undefined,

    items: (order.line_items ?? []).map((item: any) => ({
      productId: String(item.product_id ?? ''),
      productName: item.title ?? item.name,
      quantity: item.quantity,
      price: parseFloat(item.price ?? '0') || 0,
      variantId: String(item.variant_id ?? ''),
      variantName: item.variant_title
    }))
  };
}
/** note_attributes içinden anahtar adına göre değer çek */
private getNoteAttr(order: any, keys: string[]): string | undefined {
  const list: any[] = order?.note_attributes ?? [];
  const hit = list.find(x => {
    const n = String(x?.name ?? '').toLowerCase();
    return keys.some(k => n.includes(k.toLowerCase()));
  });
  const val = hit?.value;
  return (val === undefined || val === null) ? undefined : String(val);
}

/** TR telefon: 10 hane, başında 0 yok (örn: 5364621515) */
private normalizePhone10TR(raw: any): string {
  let s = String(raw ?? '').replace(/\D/g, '');
  if (!s) return '';
  // +90 / 90 öneklerini kırp
  if (s.startsWith('90') && s.length >= 12) s = s.slice(2);
  // 0 ile başlıyorsa ve 11+ hane ise ilk 0'ı kırp (0536... -> 536...)
  if (s.startsWith('0') && s.length >= 11) s = s.slice(1);
  // son 10 haneyi al
  return s.length >= 10 ? s.slice(-10) : s;
}

/** Adaylar içinden en iyi telefonu seç (öncelik: 10 hane ve 5 ile başlayan GSM) */
private pickBestPhone(order: any): string {
  const fromNotes = this.getNoteAttr(order, ['telefon', 'telefon numarası', 'phone']);

  const candidates = [
    order?.shipping_address?.phone,
    order?.billing_address?.phone,
    order?.customer?.phone,
    fromNotes
  ].filter(Boolean) as string[];

  const norm = candidates
    .map(c => this.normalizePhone10TR(c))
    .filter(x => !!x);

  // 10 haneli ve 5 ile başlayan varsa onu al
  const gsm = norm.find(x => x.length === 10 && x.startsWith('5'));
  if (gsm) return gsm;

  // 10 haneli ilkini al
  const ten = norm.find(x => x.length === 10);
  if (ten) return ten;

  // elde ne varsa en uzunu al (fallback)
  return norm.sort((a, b) => b.length - a.length)[0] ?? '';
}


  /**
   * DHL etiketli açık & unfulfilled siparişler (Shopify-only arama kısayolu ile).
   */
  getDhlOpenUnshipped(): Observable<OrderModel[]> {
    const q = 'tag:DHL AND status:open AND fulfillment_status:unfulfilled';
    return this.searchOrdersShopifyOnly(q);
  }


  /** Sadece isim + telefon için hafif sorgu (Shopify-only) */
searchOrderContactsShopifyOnly(query: string): Observable<{ name: string; phone: string }[]> {
  return this.http
    .get<any[]>(`${this.apiUrl}/Shopify/orders/search?query=${encodeURIComponent(query)}`)
    .pipe(
      map(results => (results || []).map(order => {
        const name = `${order.customer?.first_name ?? ''} ${order.customer?.last_name ?? ''}`.trim();
        const phone = order.phone || order.customer?.phone || order.shipping_address?.phone || '';
        return { name, phone };
      })),
      map(list =>
        list
          .map(x => ({
            name: (x.name ?? '').trim(),
            phone: (x.phone ?? '').toString().trim()
          }))
          .filter(x => x.name && x.phone) // boş olanları at
      )
    );
}
/** DHL → Shopify senkron job’unu manuel tetikler */
// syncNowDhlShopify(): Observable<{ message: string }> {
//   return this.http.post<{ message: string }>(`${this.apiUrl}/DhlKargo/sync-now`, {});
// }
syncNowDhlShopify(): Observable<any> {
  return this.http.post<any>(`${this.apiUrl}/DhlKargo/sync-last7days`, {});
}

}
