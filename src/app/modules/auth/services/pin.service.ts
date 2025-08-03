import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from 'src/environments/environment';

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
    .post(`${this.apiUrl}/account/verify-pin`, { pin }, { observe: 'response' })
    .pipe(map((res) => res.status === 200));
}


getPinTimeout(): Observable<number> {
  return this.http
    .get<{ timeoutInMinutes: number }>(`${this.apiUrl}/account/pin-timeout`)
    .pipe(map(res => res.timeoutInMinutes));
}

}
