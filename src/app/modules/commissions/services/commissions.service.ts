import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export interface Commission {
  orderId: string;
  amount: number;
  percentage: number;
  date: string;
}

export interface CommissionSummary {
  userId: string;
  userName: string;
  month: string;
  totalAmount: number;
  totalCommission: number;
}

@Injectable({ providedIn: 'root' })
export class CommissionsService {
  private baseUrl = `${environment.apiUrl}/Commissions`;

  constructor(private http: HttpClient) {}

  getByUser(): Observable<Commission[]> {
    return this.http.get<Commission[]>(`${this.baseUrl}/by-user`);
  }

  getMonthlySummary(): Observable<CommissionSummary[]> {
    return this.http.get<CommissionSummary[]>(`${this.baseUrl}/monthly-summary`);
  }
}

