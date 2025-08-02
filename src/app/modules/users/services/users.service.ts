import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export interface AppUser {
  id: string;
  email: string;
  role: string;
  createdAt?: string;
}

@Injectable({ providedIn: 'root' })
export class UsersService {
  private baseUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<AppUser[]> {
    return this.http.get<AppUser[]>(this.baseUrl);
  }

  create(data: Partial<AppUser>): Observable<AppUser> {
    return this.http.post<AppUser>(this.baseUrl, data);
  }

  update(id: string, data: Partial<AppUser>): Observable<AppUser> {
    return this.http.put<AppUser>(`${this.baseUrl}/${id}`, data);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
