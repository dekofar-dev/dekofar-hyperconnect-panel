import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../auth/services/auth.service';
import { UserModel } from '../../../auth/models/user.model';
import { CommissionsService, Commission } from '../../../commissions/services/commissions.service';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {
  user?: UserModel;
  commissions: Commission[] = [];
  commissionsLoading = false;

  constructor(private auth: AuthService, private commissionsService: CommissionsService) {}

  ngOnInit(): void {
    this.auth.currentUser$.subscribe((u) => {
      this.user = u ?? undefined;
      if (u) {
        this.loadCommissions();
      }
    });
  }

  get initials(): string {
    if (!this.user) return '';
    if (this.user.firstname && this.user.lastname) {
      return (this.user.firstname[0] + this.user.lastname[0]).toUpperCase();
    }
    const name = this.user.fullName ?? '';
    const parts = name.trim().split(' ');
    return parts.slice(0, 2).map(p => p.charAt(0)).join('').toUpperCase();
  }

  private loadCommissions(): void {
    this.commissionsLoading = true;
    this.commissionsService.getByUser().subscribe({
      next: (res) => {
        this.commissions = res;
        this.commissionsLoading = false;
      },
      error: () => {
        this.commissionsLoading = false;
      },
    });
  }
}
