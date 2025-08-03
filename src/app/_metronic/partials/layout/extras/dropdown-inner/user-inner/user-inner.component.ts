import { Component, HostBinding, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { AuthService, UserType } from '../../../../../../modules/auth';

@Component({
  selector: 'app-user-inner',
  templateUrl: './user-inner.component.html',
})
export class UserInnerComponent implements OnInit, OnDestroy {
  @HostBinding('class')
  class = 'menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg menu-state-primary fw-bold py-4 fs-6 w-275px';
  @HostBinding('attr.data-kt-menu') dataKtMenu = 'true';

  user$: Observable<UserType>;
  private unsubscribe: Subscription[] = [];

  constructor(private auth: AuthService) {}

  ngOnInit(): void {
    this.user$ = this.auth.currentUser$;
  }

  // Çıkış işlemi LogoutComponent üzerinden yapılır

  getInitials(firstname?: string, lastname?: string): string {
    const first = firstname?.charAt(0)?.toUpperCase() ?? '';
    const last = lastname?.charAt(0)?.toUpperCase() ?? '';
    return `${first}${last}`;
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
