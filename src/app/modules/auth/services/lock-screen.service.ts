import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { PinService } from './pin.service';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class LockScreenService {
  // Kilitli durumu localStorage üzerinden başlat
  private isLockedSubject = new BehaviorSubject<boolean>(
    localStorage.getItem('isLocked') === 'true'
  );
  isLocked$ = this.isLockedSubject.asObservable();

  private timeoutMinutes = 5;
  private timer: any;
  private attempts = 0;

  constructor(
    private pinService: PinService,
    private authService: AuthService
  ) {}

  // Başlangıçta PIN zaman aşımını al ve dinleyicileri başlat
  init(): void {
    this.pinService.getPinTimeout().subscribe((minutes) => {
      this.timeoutMinutes = minutes || 5;
      this.startListeners();
      this.resetTimer();
    });
  }

  // Kullanıcı etkileşimini dinleyerek zamanlayıcıyı sıfırla
  private startListeners(): void {
    ['mousemove', 'click', 'keydown'].forEach((e) =>
      document.addEventListener(e, () => this.resetTimer())
    );
  }

  // Zamanlayıcıyı başlat
  private resetTimer(): void {
    clearTimeout(this.timer);
    this.timer = setTimeout(() => this.lock(), this.timeoutMinutes * 60 * 1000);
  }

  // Ekranı kilitle
  lock(): void {
    this.isLockedSubject.next(true);
    localStorage.setItem('isLocked', 'true');
  }

  // Ekranın kilidini aç
  unlock(): void {
    this.isLockedSubject.next(false);
    localStorage.removeItem('isLocked');
    this.attempts = 0;
    this.resetTimer();
  }

  // PIN doğrulaması
  verifyPin(pin: string): Observable<boolean> {
    return this.pinService.verifyPin(pin).pipe(
      tap((success) => {
        if (success) {
          this.unlock();
        } else {
          this.attempts++;
          if (this.attempts >= 3) {
            this.authService.logout(); // 3 başarısız girişte çıkış yap
          }
        }
      })
    );
  }
}
