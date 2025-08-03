import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

// Yetkili IP modelini temsil eder
export interface AllowedAdminIp {
  id: number;
  ipAddress: string;
}

@Injectable({ providedIn: 'root' })
export class AllowedAdminIpsService {
  // API temel adresi
  private baseUrl = `${environment.apiUrl}/AllowedAdminIps`;

  constructor(private http: HttpClient) {}

  // Tüm IP'leri getirir
  getAll(): Observable<AllowedAdminIp[]> {
    return this.http.get<AllowedAdminIp[]>(this.baseUrl);
  }

  // Yeni IP ekler
  addIp(ipAddress: string): Observable<AllowedAdminIp> {
    return this.http.post<AllowedAdminIp>(this.baseUrl, { ipAddress });
  }

  // IP'yi siler
  removeIp(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
