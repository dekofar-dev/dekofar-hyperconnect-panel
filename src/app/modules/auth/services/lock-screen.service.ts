import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { PinService } from './pin.service';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class LockScreenService {
  private isLockedSubject = new BehaviorSubject<boolean>(false);
  isLocked$ = this.isLockedSubject.asObservable();

  private timeoutMinutes = 5;
  private timer: any;
  private attempts = 0;

  constructor(private pinService: PinService, private authService: AuthService) {}

  init(): void {
    this.pinService.getPinTimeout().subscribe((minutes) => {
      this.timeoutMinutes = minutes || 5;
      this.startListeners();
      this.resetTimer();
    });
  }

  private startListeners(): void {
    ['mousemove', 'click', 'keydown'].forEach((e) =>
      document.addEventListener(e, () => this.resetTimer())
    );
  }

  private resetTimer(): void {
    clearTimeout(this.timer);
    this.timer = setTimeout(() => this.lock(), this.timeoutMinutes * 60 * 1000);
  }

  lock(): void {
    this.isLockedSubject.next(true);
  }

  unlock(): void {
    this.isLockedSubject.next(false);
    this.attempts = 0;
    this.resetTimer();
  }

  verifyPin(pin: string): Observable<boolean> {
    return this.pinService.verifyPin(pin).pipe(
      tap((success) => {
        if (success) {
          this.unlock();
        } else {
          this.attempts++;
          if (this.attempts >= 3) {
            this.authService.logout();
          }
        }
      })
    );
  }
}
