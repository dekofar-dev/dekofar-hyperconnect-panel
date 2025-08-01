import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Order, PagedResult } from '../model/shopfy/order.model';
import { ShopifyOrderDetailDto } from '../model/shopfy/shopify-order-detail.model';

@Injectable({
  providedIn: 'root',
})
export class OrdersService {
  // ✅ Temel API adresi
  private readonly API_URL = `${environment.apiUrl}/Shopify`;

  constructor(private http: HttpClient) {}

  /**
   * Sayfalı şekilde onaylı siparişleri getirir
   */
getConfirmedOrdersPaged(limit: number, pageInfo?: string): Observable<PagedResult<Order>> {
  const params = new HttpParams()
    .set('limit', limit.toString())
    .set('pageInfo', pageInfo || '');

  return this.http.get<PagedResult<Order>>(`${this.API_URL}/orders-paged`, { params });
}


  /**
   * Sipariş detayını (ürün görselleri dahil) getirir
   */
  getOrderDetailWithImages(orderId: number): Observable<Order> {
    return this.http.get<Order>(`${this.API_URL}/order/${orderId}/with-images`);
  }

  /**
   * Siparişe yeni not ekler veya günceller
   */
  updateOrderNote(orderId: number, updatedNote: string): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/order/${orderId}/note`, {
      note: updatedNote,
    });
  }

  /**
   * Siparişin etiket bilgisini günceller
   */
  updateOrderTags(orderId: number, tags: string): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/order/${orderId}/tags`, {
      tags: tags,
    });
  }

  /**
   * Shopify bağlantısını test eder
   */
  testConnection(): Observable<any> {
    return this.http.get(`${this.API_URL}/test`);
  }

  /**
   * Sipariş ID ile ham sipariş detayını getirir (opsiyonel)
   */
  getOrderById(orderId: number): Observable<Order> {
    return this.http.get<Order>(`${this.API_URL}/order/${orderId}`);
  }

getOrderDetail(orderId: number): Observable<ShopifyOrderDetailDto> {
  return this.http.get<ShopifyOrderDetailDto>(`${this.API_URL}/order-detail/${orderId}`);
}

  /**
   * 🔍 Sipariş arama (isim, e-posta, telefon, sipariş no)
   */
  searchOrders(query: string): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.API_URL}/orders/search`, {
      params: { query },
    });
  }


}
