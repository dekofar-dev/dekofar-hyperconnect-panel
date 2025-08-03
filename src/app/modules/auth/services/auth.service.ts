import { Injectable, OnDestroy } from '@angular/core';
import { Observable, BehaviorSubject, of, Subscription } from 'rxjs';
import { map, catchError, finalize, tap } from 'rxjs/operators';
import { UserModel } from '../models/user.model';
import { Router } from '@angular/router';
import { AuthHTTPService } from './auth-http/auth-http.service';
import { WorkSessionService } from '../../work-sessions/services/work-session.service';

export type UserType = UserModel | undefined;

@Injectable({
  providedIn: 'root',
})
export class AuthService implements OnDestroy {
  private unsubscribe: Subscription[] = [];
  private authLocalStorageToken = 'auth_token';
  private authLocalStorageUser = 'auth_user';

  currentUser$: Observable<UserType>;
  isLoading$: Observable<boolean>;
  currentUserSubject: BehaviorSubject<UserType>;
  isLoadingSubject: BehaviorSubject<boolean>;

  get currentUserValue(): UserType {
    return this.currentUserSubject.value;
  }

  constructor(
    private authHttpService: AuthHTTPService,
    private router: Router,
    private workSessionService: WorkSessionService
  ) {
    this.isLoadingSubject = new BehaviorSubject<boolean>(false);
    this.currentUserSubject = new BehaviorSubject<UserType>(this.getUserInfo());
    this.currentUser$ = this.currentUserSubject.asObservable();
    this.isLoading$ = this.isLoadingSubject.asObservable();
  }

  // Kullanıcı giriş işlemi
  login(email: string, password: string): Observable<UserType> {
    this.isLoadingSubject.next(true);
    return this.authHttpService.login(email, password).pipe(
      map((auth) => {
        if (auth?.token && auth?.user) {
          this.setAuthToken(auth.token);
          this.setUserInfo(auth.user);
          this.currentUserSubject.next(auth.user);
          return auth.user;
        }
        return undefined;
      }),
      tap((user) => {
        if (user) {
          const sub = this.workSessionService
            .startSession()
            .subscribe({ error: (err) => console.error('Oturum başlatılamadı:', err) });
          this.unsubscribe.push(sub);
        }
      }),
      catchError((err) => {
        console.error('Giriş hatası:', err);
        return of(undefined);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  // Yeni kullanıcı kayıt işlemi
  register(fullName: string, email: string, password: string): Observable<UserType> {
    this.isLoadingSubject.next(true);
    return this.authHttpService.register(fullName, email, password).pipe(
      map((auth) => {
        if (auth?.token && auth?.user) {
          this.setAuthToken(auth.token);
          this.setUserInfo(auth.user);
          this.currentUserSubject.next(auth.user);
          return auth.user;
        }
        return undefined;
      }),
      catchError((err) => {
        console.error('Kayıt hatası:', err);
        return of(undefined);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  // Sunucudan kullanıcı profili alma
  getProfile(): Observable<UserType> {
    return this.authHttpService.getProfile().pipe(
      map((user) => {
        this.setUserInfo(user);
        this.currentUserSubject.next(user);
        return user;
      }),
      catchError((err) => {
        console.error('Profil bilgisi alınamadı:', err);
        this.logout();
        return of(undefined);
      })
    );
  }

  // Oturum kapatma işlemi
  logout(): void {
    const wsSub = this.workSessionService
      .endSession()
      .subscribe({ error: (err) => console.error('Oturum sonlandırılamadı:', err) });
    this.unsubscribe.push(wsSub);

    const sub = this.authHttpService
      .logout()
      .pipe(
        finalize(() => {
          localStorage.removeItem(this.authLocalStorageToken);
          localStorage.removeItem(this.authLocalStorageUser);
          this.currentUserSubject.next(undefined);
          this.router.navigate(['/auth/login']);
        })
      )
      .subscribe({ error: (err) => console.error('Çıkış hatası:', err) });
    this.unsubscribe.push(sub);
  }

  getToken(): string | null {
    return localStorage.getItem(this.authLocalStorageToken);
  }

  getAuthFromLocalStorage(): { id: string; email: string; role: string } | null {
    const authData = localStorage.getItem(this.authLocalStorageUser);
    if (!authData) return null;

    try {
      const user = JSON.parse(authData);
      return {
        id: user.id,
        email: user.email,
        role: user.role
      };
    } catch (e) {
      console.error('❌ Kullanıcı bilgisi çözümlenemedi:', e);
      return null;
    }
  }

  private setAuthToken(token: string): void {
    localStorage.setItem(this.authLocalStorageToken, token);
  }

  private setUserInfo(user: UserModel): void {
    localStorage.setItem(this.authLocalStorageUser, JSON.stringify(user));
  }

  private getUserInfo(): UserModel | undefined {
    const userJson = localStorage.getItem(this.authLocalStorageUser);
    return userJson ? JSON.parse(userJson) : undefined;
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
