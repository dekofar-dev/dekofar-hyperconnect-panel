import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export interface Discount {
  id: number;
  name: string;
  type: string;
  value: number;
  status: string | number;
  createdBy?: string;
}

@Injectable({ providedIn: 'root' })
export class DiscountService {
  private baseUrl = `${environment.apiUrl}/discounts`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Discount[]> {
    return this.http.get<Discount[]>(this.baseUrl);
  }

  create(data: Partial<Discount>): Observable<Discount> {
    return this.http.post<Discount>(this.baseUrl, data);
  }

  update(id: number, data: Partial<Discount>): Observable<Discount> {
    return this.http.put<Discount>(`${this.baseUrl}/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
