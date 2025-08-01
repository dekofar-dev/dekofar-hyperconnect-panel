import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { AuthModel } from '../../models/auth.model';
import { UserModel } from '../../models/user.model';

const API_USERS_URL = `${environment.apiUrl}/auth`;

@Injectable({
  providedIn: 'root',
})
export class AuthHTTPService {
  [x: string]: any;
  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<AuthModel> {
    return this.http.post<AuthModel>(`${API_USERS_URL}/login`, {
      email,
      password,
    });
  }
getProfile(): Observable<UserModel> {
  return this.http.get<UserModel>(`${this.API_URL}/auth/profile`);
}

  getUserByToken(token: string): Observable<UserModel> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<UserModel>(`${API_USERS_URL}/profile`, { headers });
  }
}
