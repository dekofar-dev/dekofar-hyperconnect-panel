import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { UserModel } from '../../models/user.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  isLoading$!: Observable<boolean>;
  hasError: boolean = false;
  errorMessage: string = '';

  defaultAuth = {
    email: '',
    password: '',
  };

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isLoading$ = this.authService.isLoading$;
    this.initForm();

    if (this.authService.currentUserValue) {
      this.router.navigate(['/']);
    }
  }

  initForm(): void {
    this.loginForm = this.fb.group({
      email: [this.defaultAuth.email, [Validators.required, Validators.email]],
      password: [this.defaultAuth.password, [Validators.required]],
    });
  }

  submit(): void {
    this.hasError = false;
    this.errorMessage = '';

    const { email, password } = this.loginForm.value;

    if (!email || !password) {
      this.hasError = true;
      this.errorMessage = 'E-posta ve şifre girilmelidir.';
      return;
    }

    this.authService.login(email, password).pipe(first()).subscribe({
      next: (user: UserModel | undefined) => {
        if (user) {
          this.router.navigate(['/']);
        } else {
          this.hasError = true;
          this.errorMessage = 'Giriş başarısız.';
        }
      },
      error: (err) => {
        this.hasError = true;
        this.errorMessage = err?.error?.message || 'Sunucu hatası';
        console.error('Login error:', err);
      }
    });
  }
}
