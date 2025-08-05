import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /** Shopify ürünlerini getirir */
  getAllProducts(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Shopify/products`);
  }

  /** Belirli bir ürünün varyantlarını getirir (opsiyonel) */
  getVariants(productId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Shopify/products/${productId}/variants`);
  }
}
