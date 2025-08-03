import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
} from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class RoleGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    // Route üzerinden izin verilen rolleri al
    const allowedRoles = route.data['roles'] as string[] | string | undefined;
    const currentUser = this.authService.currentUserValue;

    // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
    if (!currentUser) {
      this.router.navigate(['/auth/login'], {
        queryParams: { returnUrl: state.url },
      });
      return false;
    }

    // Roller belirtilmemişse erişime izin ver
    if (!allowedRoles) {
      return true;
    }

    // Kullanıcının rolü izin verilenler arasında mı?
    const roles = Array.isArray(allowedRoles) ? allowedRoles : [allowedRoles];
    if (roles.includes(currentUser.role)) {
      return true;
    }

    // Yetkisiz erişimde ana sayfaya yönlendir
    this.router.navigate(['/']);
    return false;
  }
}
