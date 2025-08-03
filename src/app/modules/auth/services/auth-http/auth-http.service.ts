// src/app/modules/auth/services/auth-http/auth-http.service.ts

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { AuthModel } from '../../models/auth.model';
import { UserModel } from '../../models/user.model';

// API temel adresi (environment üzerinden)
const API_USERS_URL = `${environment.apiUrl}/Auth`;

@Injectable({
  providedIn: 'root',
})
export class AuthHTTPService {
  constructor(private http: HttpClient) {}

  // Kullanıcı giriş isteği
  login(email: string, password: string): Observable<AuthModel> {
    return this.http.post<AuthModel>(`${API_USERS_URL}/login`, {
      email,
      password,
    });
  }

  // Yeni kullanıcı kayıt isteği
  register(fullName: string, email: string, password: string): Observable<AuthModel> {
    return this.http.post<AuthModel>(`${API_USERS_URL}/register`, {
      fullName,
      email,
      password,
    });
  }

  // Oturumu sonlandırma isteği
  logout(): Observable<void> {
    return this.http.post<void>(`${API_USERS_URL}/logout`, {});
  }

  // Oturum açmış kullanıcının profilini getirme
  getProfile(): Observable<UserModel> {
    return this.http.get<UserModel>(`${API_USERS_URL}/me`);
  }
}
