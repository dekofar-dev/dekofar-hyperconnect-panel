import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { UserModel } from '../../models/user.model';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
})
export class ProfileComponent implements OnInit {
  user?: UserModel; // Giriş yapan kullanıcının bilgileri

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    // LocalStorage'daki kullanıcı bilgilerini al
    this.user = this.authService.currentUserValue;

    // Kullanıcı yoksa sunucudan profili çek
    if (!this.user) {
      this.authService.getProfile().subscribe((user) => (this.user = user));
    }
  }
}

