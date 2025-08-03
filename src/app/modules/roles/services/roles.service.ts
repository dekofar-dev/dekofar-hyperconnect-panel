import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

// Rol modelini temsil eder
export interface Role {
  id: number;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class RolesService {
  // API temel adresi
  private baseUrl = `${environment.apiUrl}/Roles`;

  constructor(private http: HttpClient) {}

  // Tüm rolleri getir
  getAllRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(this.baseUrl);
  }

  // Yeni rol oluştur
  createRole(name: string): Observable<Role> {
    return this.http.post<Role>(this.baseUrl, { name });
  }

  // Rol sil
  deleteRole(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  // Belirli bir kullanıcıya rol ata
  assignRoleToUser(userId: string, roleName: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/assign-to-user`, {
      userId,
      roleName,
    });
  }
}
