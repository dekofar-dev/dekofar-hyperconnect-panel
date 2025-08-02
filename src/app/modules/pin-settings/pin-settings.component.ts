import { Component } from '@angular/core';
import { PinService } from '../auth/services/pin.service';

@Component({
  selector: 'app-pin-settings',
  templateUrl: './pin-settings.component.html'
})
export class PinSettingsComponent {
  pin = '';
  confirmPin = '';
  message = '';
  error = '';

  constructor(private pinService: PinService) {}

  allowOnlyDigits(event: KeyboardEvent): void {
    if (!/\d/.test(event.key)) {
      event.preventDefault();
    }
  }

  submit(): void {
    this.error = '';
    this.message = '';
    if (this.pin.length !== 4 || this.pin !== this.confirmPin) {
      this.error = 'PIN uyumsuz';
      return;
    }
    this.pinService.setPin(this.pin).subscribe({
      next: () => {
        this.message = 'PIN güncellendi';
        this.pin = '';
        this.confirmPin = '';
      },
      error: () => {
        this.error = 'PIN ayarlanamadı';
      }
    });
  }
}
