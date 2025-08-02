import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PinService {
  private apiUrl = environment.apiUrl;
  constructor(private http: HttpClient) {}

  setPin(pin: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/Auth/set-pin`, { pin });
  }

  verifyPin(pin: string): Observable<boolean> {
    return this.http
      .post<{ success: boolean }>(`${this.apiUrl}/Auth/verify-pin`, { pin })
      .pipe(map((res) => res.success));
  }

  getPinTimeout(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/AppSettings/pinTimeout`);
  }
}
