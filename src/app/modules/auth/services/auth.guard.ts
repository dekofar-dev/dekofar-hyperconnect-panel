import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    // Kullanıcı bilgisi mevcutsa erişime izin ver
    const currentUser = this.authService.currentUserValue;
    if (currentUser) {
      return true;
    }

    // Kullanıcı yoksa çıkış yap ve giriş sayfasına yönlendir
    this.authService.logout();
    return false;
  }
}
