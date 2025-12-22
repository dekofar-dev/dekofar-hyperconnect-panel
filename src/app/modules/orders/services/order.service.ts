// src/app/modules/orders/services/order.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable, forkJoin, map, switchMap, of } from 'rxjs';
import { OrderModel } from '../models/order.model';
import { OrderItemSummary } from '../models/order-item-summary.model';
import { OrderTagSummary } from '../models/order-tag-summary.model';
import { ProductVariantSummary } from '../models/product-variant-summary.model';


@Injectable({ providedIn: 'root' })
export class OrderService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /** Shopify ve manuel siparişleri birleştirip tarih sırasına göre döner (sayfalı Shopify). */




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



  getManualOrderById(id: string): Observable<OrderModel> {
    return this.http.get<any>(`${this.apiUrl}/manual-orders/${id}`).pipe(
      map(data => this.mapManualOrder(data))
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

 



  // =====================================================
  // 🔹 ORDERS – GENEL
  // =====================================================

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
        const shopifyOrders = (shopifyRes.items || []).map(o => this.mapShopifyOrder(o));
        const formattedManual = (manualOrders || []).map(o => ({
          ...o,
          source: 'Manual' as const,
          currency: 'TRY'
        }));

        return {
          items: [...formattedManual, ...shopifyOrders].sort(
            (a, b) => +new Date(b.createdAt) - +new Date(a.createdAt)
          ),
          nextPageInfo: shopifyRes.nextPageInfo ?? undefined,
          previousPageInfo: direction === 'prev' ? pageInfo : undefined
        };
      })
    );
  }






  // =====================================================
  // 🔹 SHOPIFY REPORTS (YENİ – DOĞRU)
  // =====================================================

  /**
   * 🔹 Açık / gönderilmemiş siparişlerde
   * Ürün → Varyant → Toplam Adet özeti
   */
  getOpenOrderProductVariantSummary(): Observable<OrderItemSummary[]> {
    return this.http.get<OrderItemSummary[]>(
      `${this.apiUrl}/integrations/shopify/reports/orders/open/product-variant-summary`
    );
  }
























  // =====================================================
  // 🔹 SEARCH & LIST
  // =====================================================

  searchOrdersDetailedFrontOnly(
    query: string,
    max = 50
  ): Observable<OrderModel[]> {
    const url = `${this.apiUrl}/Shopify/orders/search?query=${encodeURIComponent(query)}`;

    return this.http.get<any[]>(url).pipe(
      map(list => (list || []).map(o => o.id)),
      map(ids => Array.from(new Set(ids)).slice(0, max)),
      switchMap(ids => {
        if (!ids.length) return of<OrderModel[]>([]);
        return forkJoin(ids.map(id => this.getShopifyOrderById(String(id))));
      }),
      map(items =>
        items.sort(
          (a, b) =>
            +new Date(b.createdAt) - +new Date(a.createdAt)
        )
      )
    );
  }

  searchOrders(query: string): Observable<OrderModel[]> {
    const manual$ = this.http
      .get<any[]>(`${this.apiUrl}/manual-orders/search?query=${encodeURIComponent(query)}`)
      .pipe(map(r => (r || []).map(o => this.mapManualOrder(o))));

    const shopify$ = this.http
      .get<any[]>(`${this.apiUrl}/Shopify/orders/search?query=${encodeURIComponent(query)}`)
      .pipe(map(r => (r || []).map(o => this.mapShopifyOrder(o))));

    return forkJoin([manual$, shopify$]).pipe(
      map(([m, s]) =>
        [...m, ...s].sort(
          (a, b) =>
            +new Date(b.createdAt) - +new Date(a.createdAt)
        )
      )
    );
  }

  searchOrdersShopifyOnly(query: string): Observable<OrderModel[]> {
    return this.http
      .get<any[]>(`${this.apiUrl}/Shopify/orders/search?query=${encodeURIComponent(query)}`)
      .pipe(
        map(r => (r || []).map(o => this.mapShopifyOrder(o))),
        map(list =>
          list.sort(
            (a, b) =>
              +new Date(b.createdAt) - +new Date(a.createdAt)
          )
        )
      );
  }

  // =====================================================
  // 🔹 ORDERS
  // =====================================================

  getManualOrders(): Observable<OrderModel[]> {
    return this.http
      .get<any[]>(`${this.apiUrl}/manual-orders`)
      .pipe(map(r => (r || []).map(o => this.mapManualOrder(o))));
  }

  getShopifyOrders(): Observable<OrderModel[]> {
    return this.http
      .get<{ items: any[] }>(`${this.apiUrl}/Shopify/orders-paged?limit=100`)
      .pipe(map(r => (r.items || []).map(o => this.mapShopifyOrder(o))));
  }

  getShopifyOrderById(id: string): Observable<OrderModel> {
    return this.http
      .get<any>(`${this.apiUrl}/Shopify/orders/${id}`)
      .pipe(map(o => this.mapShopifyOrder(o)));
  }

  // =====================================================
  // 🔹 SHOPIFY REPORTS (ÖNEMLİ KISIM)
  // =====================================================

  /**
   * ✅ ÜRÜN → VARYANT → TOPLAM ADET
   * Tag opsiyonel
   */
  getProductVariantSummary(
    tag?: string
  ): Observable<ProductVariantSummary[]> {
    const q = tag ? `?tag=${encodeURIComponent(tag)}` : '';

    return this.http.get<ProductVariantSummary[]>(
      `${this.apiUrl}/integrations/shopify/reports/orders/open/product-variant-summary${q}`
    );
  }

  /**
   * 🔹 Açık sipariş etiketleri
   */
  getOpenOrderTagSummary(): Observable<OrderTagSummary[]> {
    return this.http.get<OrderTagSummary[]>(
      `${this.apiUrl}/integrations/shopify/reports/orders/open/tag-summary`
    );
  }

  /**
   * 🔹 Dashboard paralel yükleme
   */
  getOpenOrderDashboardData(): Observable<{
    products: ProductVariantSummary[];
    tags: OrderTagSummary[];
  }> {
    return forkJoin({
      products: this.getProductVariantSummary(),
      tags: this.getOpenOrderTagSummary()
    });
  }

  // =====================================================
  // 🔹 MAPPERS
  // =====================================================

  private mapManualOrder(order: any): OrderModel {
    return {
      ...order,
      id: String(order.id),
      source: 'Manual',
      currency: 'TRY'
    };
  }

  private mapShopifyOrder(order: any): OrderModel {
    const addr = order.shipping_address ?? order.billing_address ?? {};

    return {
      id: String(order.id),
      orderNumber: order.order_number ?? order.name,
      source: 'Shopify',
      createdAt: order.created_at,
      status: order.financial_status,
      customerName: `${order.customer?.first_name ?? ''} ${order.customer?.last_name ?? ''}`.trim(),
      phone: addr.phone ?? order.customer?.phone ?? '',
      email: order.email ?? '',
      address: [addr.address1, addr.address2].filter(Boolean).join(' '),
      city: addr.city ?? '',
      district: addr.province ?? '',
      paymentType: 'Kredi Kartı',
      orderNote: order.note,
      totalAmount: parseFloat(order.total_price ?? '0'),
      currency: order.currency ?? 'TRY',
      items: (order.line_items ?? []).map((i: any) => ({
        productId: String(i.product_id),
        productName: i.title,
        quantity: i.quantity,
        price: parseFloat(i.price ?? '0'),
        variantId: String(i.variant_id),
        variantName: i.variant_title
      }))
    };
  }
}





























