import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

// İzin modelini temsil eder
export interface Permission {
  id: number;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class PermissionsService {
  // API temel adresi
  private baseUrl = `${environment.apiUrl}/Permissions`;

  constructor(private http: HttpClient) {}

  // Tüm izinleri getir
  getAllPermissions(): Observable<Permission[]> {
    return this.http.get<Permission[]>(this.baseUrl);
  }

  // Bir role izin ata
  assignPermissionsToRole(roleId: number, permissions: string[]): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/assign-to-role`, {
      roleId,
      permissions,
    });
  }
}
