import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export interface SupportCategory {
  id: number;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class SupportCategoryService {
  private baseUrl = `${environment.apiUrl}/support-categories`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<SupportCategory[]> {
    return this.http.get<SupportCategory[]>(this.baseUrl);
  }

  create(data: Partial<SupportCategory>): Observable<SupportCategory> {
    return this.http.post<SupportCategory>(this.baseUrl, data);
  }

  update(id: number, data: Partial<SupportCategory>): Observable<SupportCategory> {
    return this.http.put<SupportCategory>(`${this.baseUrl}/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
