import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../modules/auth/services/auth.service';
import { UserModel } from '../../modules/auth/models/user.model';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
})
export class ProfileComponent implements OnInit {
  user?: UserModel;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe((u) => {
      this.user = u ?? undefined;
    });
  }

  get initials(): string {
    if (!this.user) return '?';

    const name = this.user.fullName?.trim() ?? '';
    const parts = name.split(' ');

    if (parts.length >= 2) {
      return (parts[0][0] + parts[1][0]).toUpperCase();
    } else if (parts.length === 1 && parts[0].length > 0) {
      return parts[0][0].toUpperCase();
    }

    return '?';
  }
}
