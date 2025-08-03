import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SupportCategory } from '../models/support-category.model';

@Injectable({ providedIn: 'root' })
export class SupportCategoryService {
  private baseUrl = `${environment.apiUrl}/support-categories`;

  constructor(private http: HttpClient) {}

  /** 
   * ✅ Tüm destek kategorilerini getirir 
   */
  getAll(): Observable<SupportCategory[]> {
    return this.http.get<SupportCategory[]>(this.baseUrl);
  }

  /** 
   * ✅ Yeni bir destek kategorisi oluşturur 
   */
  create(data: { name: string }): Observable<string> {
    return this.http.post<string>(this.baseUrl, data);
  }

  /** 
   * ✅ Destek kategorisini günceller 
   */
  update(id: string, data: { name: string }): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, { ...data, id });
  }

  /** 
   * ✅ Belirtilen ID'ye sahip kategoriyi siler 
   */
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  /** 
   * ✅ Kategoriye roller atar 
   */
  assignRoles(id: string, roles: string[]): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/roles`, { roles });
  }
}
