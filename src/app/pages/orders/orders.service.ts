import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { OrderSummary } from './order.model';

@Injectable({ providedIn: 'root' })
export class OrdersService {
  private readonly SHOPIFY_URL = `${environment.apiUrl}/ShopifyOrders`;
  private readonly MANUAL_URL = `${environment.apiUrl}/ManualOrders`;

  constructor(private http: HttpClient) {}

  getShopifyOrders(): Observable<OrderSummary[]> {
    return this.http.get<OrderSummary[]>(this.SHOPIFY_URL);
  }

  getManualOrders(): Observable<OrderSummary[]> {
    return this.http.get<OrderSummary[]>(this.MANUAL_URL);
  }
}
