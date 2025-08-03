import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserModel } from '../../auth/models/user.model';

// Kullanıcı işlemleri için servis
@Injectable({ providedIn: 'root' })
export class UsersService {
  // API için temel adres
  private apiUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) {}

  // Mevcut kullanıcı bilgisini getirir
  getMe(): Observable<UserModel> {
    return this.http.get<UserModel>(`${this.apiUrl}/me`);
  }

  // Mevcut kullanıcı bilgisini günceller
  updateMe(data: Partial<UserModel>): Observable<UserModel> {
    return this.http.put<UserModel>(`${this.apiUrl}/me`, data);
  }

  // Şifre değiştirme isteği gönderir
  changePassword(currentPassword: string, newPassword: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/change-password`, {
      currentPassword,
      newPassword,
    });
  }

  // Profil resmini yükler
  uploadAvatar(userId: string, file: File): Observable<{ avatarUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ avatarUrl: string }>(`${this.apiUrl}/${userId}/avatar`, formData);
  }

  // Profil resmini kaldırır
  removeAvatar(userId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${userId}/avatar`);
  }

  // 4 haneli PIN ayarlar
  setPin(pin: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/set-pin`, { pin });
  }

  // 4 haneli PIN doğrular
  verifyPin(pin: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/verify-pin`, { pin });
  }
}
