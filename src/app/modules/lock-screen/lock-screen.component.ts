import { Component } from '@angular/core';
import { AuthService } from '../auth/services/auth.service';
import { LockScreenService } from '../auth/services/lock-screen.service';
import { UserModel } from '../auth/models/user.model';

@Component({
  selector: 'app-lock-screen',
  templateUrl: './lock-screen.component.html',
  styleUrls: ['./lock-screen.component.scss']
})
export class LockScreenComponent {
  pin = '';
  error = '';
  user?: UserModel;

  constructor(
    private authService: AuthService,
    private lockService: LockScreenService
  ) {
    this.authService.currentUser$.subscribe(u => (this.user = u));
  }

  get initials(): string {
    if (!this.user?.fullName) return '';
    return this.user.fullName
      .split(' ')
      .map(n => n.charAt(0))
      .join('')
      .substring(0, 2)
      .toUpperCase();
  }

  allowOnlyDigits(event: KeyboardEvent): void {
    if (!/\d/.test(event.key)) {
      event.preventDefault();
    }
  }

  submit(): void {
    if (this.pin.length !== 4) {
      return;
    }
    this.lockService.verifyPin(this.pin).subscribe(success => {
      if (!success) {
        this.error = 'PIN yanlış';
      } else {
        this.pin = '';
        this.error = '';
      }
    });
  }
}
