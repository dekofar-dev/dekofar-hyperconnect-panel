import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private tokenKey = 'auth_token';

  constructor(private router: Router) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const token = localStorage.getItem(this.tokenKey);

    // Eğer token varsa Authorization header'ı ekle
    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });
    }

    // 401 hatasını yakala ve login'e yönlendir (opsiyonel)
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          // Oturum süresi doldu veya yetkisiz -> login sayfasına yönlendir
          localStorage.removeItem(this.tokenKey);
          this.router.navigate(['/auth/login']);
        }
        return throwError(() => error);
      })
    );
  }
}
