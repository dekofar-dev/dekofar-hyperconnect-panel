import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable, forkJoin, map } from 'rxjs';
import { OrderModel } from '../models/order.model';

@Injectable({
  providedIn: 'root' // Bu servis tüm uygulamada singleton olarak kullanılabilir
})
export class OrderService {
  private apiUrl = environment.apiUrl; // API base URL ortam değişkeninden alınır

  constructor(private http: HttpClient) {}

  /**
   * Shopify ve manuel siparişleri birleştirip tarih sırasına göre sıralı şekilde döner.
   * @param limit - Shopify siparişlerinde kaç adet alınacağı
   * @param pageInfo - Sayfalama bilgisi (Shopify için geçerli)
   * @param direction - Sayfalama yönü (varsayılan: 'next')
   */
  getCombinedOrders(
    limit: number,
    pageInfo?: string,
    direction: 'next' | 'prev' = 'next'
  ): Observable<{
    items: OrderModel[];
    nextPageInfo?: string;
    previousPageInfo?: string;
  }> {
    // Shopify siparişlerini sayfalı şekilde çek
    const shopify$ = this.http.get<{ items: any[]; nextPageInfo?: string | null }>(
      `${this.apiUrl}/Shopify/orders-paged?limit=${limit}${pageInfo ? `&pageInfo=${encodeURIComponent(pageInfo)}` : ''}`
    );

    // Manuel siparişleri çek
    const manual$ = this.getManualOrders();

    // Her iki istek tamamlandığında işlem yap
    return forkJoin([shopify$, manual$]).pipe(
      map(([shopifyRes, manualOrders]) => {
        // Shopify siparişlerini modele dönüştür
        const shopifyOrders: OrderModel[] = shopifyRes.items.map((order: any) => this.mapShopifyOrder(order));

        // Manuel siparişleri modele dönüştür ve ek alanları hazırla
        const formattedManualOrders: OrderModel[] = manualOrders.map(order => ({
          ...order,
          source: 'Manual' as const,
          currency: 'TRY',
          createdByAvatarUrl: order.createdByAvatarUrl ?? undefined
        }));

        // Her iki siparişi birleştirip tarihe göre sıralıyoruz (yeniden eskiye)
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
   * Sipariş arama işlemi. Shopify ve manuel siparişlerde arama yapılır.
   * @param query - Arama sorgusu
   */
searchOrders(query: string): Observable<OrderModel[]> {
  const manual$ = this.http
    .get<any[]>(`${this.apiUrl}/manual-orders/search?query=${encodeURIComponent(query)}`)
    .pipe(map(results => results.map(order => this.mapManualOrder(order))));

  const shopify$ = this.http
    .get<any[]>(`${this.apiUrl}/Shopify/orders/search?query=${encodeURIComponent(query)}`)
    .pipe(map(results => results.map(order => this.mapShopifyOrder(order))));

  return forkJoin([manual$, shopify$]).pipe(
    map(([manualOrders, shopifyOrders]) =>
      [...manualOrders, ...shopifyOrders].sort(
        (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      )
    )
  );
}


  /**
   * Tüm manuel ve Shopify siparişlerini çeker (sayfalama yok)
   */
  getAllOrders(): Observable<OrderModel[]> {
    return forkJoin([
      this.getManualOrders(),
      this.getShopifyOrders()
    ]).pipe(
      map(([manualOrders, shopifyOrders]) =>
        [...manualOrders, ...shopifyOrders].sort(
          (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        )
      )
    );
  }

  /**
   * Tüm manuel siparişleri getirir
   */
  getManualOrders(): Observable<OrderModel[]> {
    return this.http.get<any[]>(`${this.apiUrl}/manual-orders`).pipe(
      map(response => response.map(order => this.mapManualOrder(order)))
    );
  }

  /**
   * Shopify üzerinden siparişleri getirir (limit 100, sayfalama yok)
   */
  getShopifyOrders(): Observable<OrderModel[]> {
    const url = `${this.apiUrl}/Shopify/orders-paged?limit=100`;
    return this.http.get<{ items: any[] }>(url).pipe(
      map(response => response.items.map(order => this.mapShopifyOrder(order)))
    );
  }

  /**
   * Belirli bir manuel siparişi ID'ye göre getirir
   */
  getManualOrderById(id: string): Observable<OrderModel> {
    return this.http.get<any>(`${this.apiUrl}/manual-orders/${id}`).pipe(
      map(data => this.mapManualOrder(data))
    );
  }

  /**
   * Belirli bir Shopify siparişi ID'ye göre getirir
   */
  getShopifyOrderById(id: string): Observable<OrderModel> {
    return this.http.get<any>(`${this.apiUrl}/shopify-orders/${id}`).pipe(
      map(data => this.mapShopifyOrder(data))
    );
  }

  /**
   * Yeni bir manuel sipariş oluşturur
   * @param order - Sipariş bilgileri ve ürün listesi
   */
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
   * API'den gelen manuel siparişi OrderModel formatına dönüştürür
   */
  private mapManualOrder(order: any): OrderModel {
    return {
      id: order.id.toString(),
      orderNumber: order.orderNumber ?? order.id.toString(),
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
      items: order.items?.map((item: any) => ({
        productId: item.productId,
        productName: item.productName,
        quantity: item.quantity,
        price: item.price,
        variantId: item.variantId,
        variantName: item.variantName
      })) ?? []
    };
  }

  /**
   * Shopify API'den gelen sipariş verisini OrderModel formatına dönüştürür
   */
  private mapShopifyOrder(order: any): OrderModel {
    return {
      id: order.id.toString(),
      orderNumber: order.order_number,
      source: 'Shopify',
      createdAt: order.created_at,
      status: order.financial_status === 'paid'
        ? 'Tamamlandı'
        : order.financial_status === 'pending'
          ? 'Beklemede'
          : 'Bilinmiyor',
      customerName: `${order.customer?.first_name ?? ''} ${order.customer?.last_name ?? ''}`.trim() || 'Shopify Müşterisi',
      phone: order.customer?.phone,
      email: order.customer?.email,
      address: order.shipping_address?.address1,
      city: order.shipping_address?.city,
      district: order.shipping_address?.province,
      paymentType: 'Kredi Kartı',
      orderNote: order.note,
      totalAmount: parseFloat(order.total_price),
      currency: order.currency ?? 'TRY',
      createdByAvatarUrl: undefined,
      items: order.line_items?.map((item: any) => ({
        productId: item.product_id,
        productName: item.name,
        quantity: item.quantity,
        price: parseFloat(item.price),
        variantId: item.variant_id,
        variantName: item.variant_title
      })) ?? []
    };
  }
}
