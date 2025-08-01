import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../modules/auth/services/auth.service';
import { UserModel } from '../../modules/auth/models/user.model';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
})
export class ProfileComponent implements OnInit {
  user: UserModel = {
    fullName: '',
    email: '',
    role: '',
    id: ''
  };

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe((u) => {
      if (u) {
        this.user = u;
      }
    });
  }

  getInitials(): string {
    const name = this.user.fullName ?? '';
    const parts = name.trim().split(' ');
    return parts.length >= 2
      ? parts[0][0].toUpperCase() + parts[1][0].toUpperCase()
      : parts[0][0]?.toUpperCase() ?? '?';
  }
}
