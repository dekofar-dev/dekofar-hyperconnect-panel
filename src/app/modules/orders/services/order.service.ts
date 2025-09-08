// src/app/modules/orders/services/order.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable, forkJoin, map, switchMap, of } from 'rxjs';
import { OrderModel } from '../models/order.model';
import { OrderItemSummary } from '../models/order-item-summary.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /** Shopify ve manuel siparişleri birleştirip tarih sırasına göre döner (sayfalı Shopify). */
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

  /** (Opsiyonel) Front-end’de detay toplama: arama → ID listesi → tekil detail çağrıları. */
  searchOrdersDetailedFrontOnly(query: string, max = 50): Observable<OrderModel[]> {
    const url = `${this.apiUrl}/Shopify/orders/search?query=${encodeURIComponent(query)}`;
    return this.http.get<any[]>(url).pipe(
      map(list => (list || []).map((o: any) => o.id)),
      map(ids => Array.from(new Set(ids)).slice(0, max)),
      switchMap(ids => {
        if (!ids.length) return of<OrderModel[]>([]);
        const calls = ids.map((id: any) => this.getShopifyOrderById(String(id)));
        return forkJoin(calls);
      }),
      map(items => items.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()))
    );
  }

  /** Manuel + Shopify araması (genel). */
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

  /** (Kısayol) Sadece Shopify araması — Excel/export senaryoları için idealdir. */
  searchOrdersShopifyOnly(query: string): Observable<OrderModel[]> {
    return this.http
      .get<any[]>(`${this.apiUrl}/Shopify/orders/search?query=${encodeURIComponent(query)}`)
      .pipe(
        map(results => (results || []).map(order => this.mapShopifyOrder(order))),
        map(list => list.sort((a, b) => +new Date(b.createdAt) - +new Date(a.createdAt)))
      );
  }

  /** (Opsiyonel) Hafif liste: backend’in /orders/search-lite’i */
  searchOrdersLite(query: string): Observable<OrderModel[]> {
    return this.http
      .get<any[]>(`${this.apiUrl}/Shopify/orders/search-lite?query=${encodeURIComponent(query)}`)
      .pipe(map(results => (results || []).map(order => this.mapShopifyOrder(order))));
  }

  /** Tüm siparişleri al (sayfalama yok). */
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

  /** DÜZELTİLDİ: Tekil Shopify sipariş endpoint’i → /api/Shopify/orders/{id} */
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

  /** DHL etiketli açık & unfulfilled siparişler */
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
            .filter(x => x.name && x.phone)
        )
      );
  }

  /** Shopify ürün bazlı özet (varsayılan son 30 gün) */
  getShopifyOrderItemsSummary(params?: {
    days?: number;
    statusCsv?: string;        // "open"
    financialCsv?: string;     // "pending,authorized,paid,partially_paid,partially_refunded"
    fulfillmentCsv?: string;   // "unfulfilled,partial"
  }): Observable<OrderItemSummary[]> {
    const q = new URLSearchParams();
    q.set('days', String(params?.days ?? 30));
    q.set('status', params?.statusCsv ?? 'open');
    q.set('financial', params?.financialCsv ?? 'pending,authorized,paid,partially_paid,partially_refunded');
    q.set('fulfillment', params?.fulfillmentCsv ?? 'unfulfilled,partial');
    return this.http.get<OrderItemSummary[]>(`${this.apiUrl}/Shopify/order-items/summary?${q.toString()}`);
  }

  /** 🔹 Yeni: tarih aralığı verip özet alma (backend: /order-items/summary-by-range) */
  getShopifyOrderItemsSummaryByRange(opts: {
    start: Date | string;
    end: Date | string;
    statusCsv?: string;
    financialCsv?: string;
    fulfillmentCsv?: string;
  }): Observable<OrderItemSummary[]> {
    const q = new URLSearchParams();
    q.set('start', typeof opts.start === 'string' ? opts.start : opts.start.toISOString());
    q.set('end',   typeof opts.end   === 'string' ? opts.end   : opts.end.toISOString());
    q.set('status',      opts.statusCsv     ?? 'open');
    q.set('financial',   opts.financialCsv  ?? 'pending,authorized,paid,partially_paid,partially_refunded');
    q.set('fulfillment', opts.fulfillmentCsv?? 'unfulfilled,partial');
    return this.http.get<OrderItemSummary[]>(`${this.apiUrl}/Shopify/order-items/summary-by-range?${q.toString()}`);
  }

  /** 🔹 Yeni: server-side cache temizle (tüm ilgili bellek cache’leri) */
  clearShopifyCaches(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/Shopify/orders/clear-cache`, {});
  }

  /** 🔹 Yeni: cursor bazlı açık siparişler (ilk sayfa için pageInfo vermeyin) */
  getOpenOrdersWithCursor(limit = 20, pageInfo?: string): Observable<{ items: OrderModel[]; nextPageInfo?: string }> {
    const q = new URLSearchParams();
    q.set('limit', String(limit));
    if (pageInfo) q.set('pageInfo', pageInfo);
    return this.http
      .get<{ items: any[]; nextPageInfo?: string }>(`${this.apiUrl}/Shopify/orders-open-cursor?${q.toString()}`)
      .pipe(map(res => ({
        items: (res.items || []).map(o => this.mapShopifyOrder(o)),
        nextPageInfo: res.nextPageInfo
      })));
  }

  // ------------ Mappers & yardımcılar ------------

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

    const addr = order.shipping_address ?? order.billing_address ?? {};
    const address = [addr.address1, addr.address2].filter(Boolean).join(' ').trim();

    const noteIl   = this.getNoteAttr(order, ['şehir', 'sehir', 'il']);
    const noteIlce = this.getNoteAttr(order, ['ilçe', 'ilce']);

    const city     = (noteIl   ?? addr.city     ?? '').toString().trim();   // İL
    const district = (noteIlce ?? addr.province ?? '').toString().trim();   // İLÇE

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
      city,
      district,

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

  private getNoteAttr(order: any, keys: string[]): string | undefined {
    const list: any[] = order?.note_attributes ?? [];
    const hit = list.find(x => {
      const n = String(x?.name ?? '').toLowerCase();
      return keys.some(k => n.includes(k.toLowerCase()));
    });
    const val = hit?.value;
    return (val === undefined || val === null) ? undefined : String(val);
  }

  private normalizePhone10TR(raw: any): string {
    let s = String(raw ?? '').replace(/\D/g, '');
    if (!s) return '';
    if (s.startsWith('90') && s.length >= 12) s = s.slice(2);
    if (s.startsWith('0') && s.length >= 11) s = s.slice(1);
    return s.length >= 10 ? s.slice(-10) : s;
  }

  private pickBestPhone(order: any): string {
    const fromNotes = this.getNoteAttr(order, ['telefon', 'telefon numarası', 'phone']);
    const candidates = [
      order?.shipping_address?.phone,
      order?.billing_address?.phone,
      order?.customer?.phone,
      fromNotes
    ].filter(Boolean) as string[];

    const norm = candidates.map(c => this.normalizePhone10TR(c)).filter(x => !!x);
    const gsm = norm.find(x => x.length === 10 && x.startsWith('5'));
    if (gsm) return gsm;
    const ten = norm.find(x => x.length === 10);
    if (ten) return ten;
    return norm.sort((a, b) => b.length - a.length)[0] ?? '';
  }

  /** DHL → Shopify senkron job’unu manuel tetikler */
  syncNowDhlShopify(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/DhlKargo/sync-last7days`, {});
  }
}
