import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { first } from 'rxjs/operators';
import { AuthService } from '../../services/auth.service';
import { UserModel } from '../../models/user.model';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup; // Kayıt formu
  isLoading$!: Observable<boolean>; // Yüklenme durumu
  hasError: boolean = false; // Hata durumu
  errorMessage: string = ''; // Hata mesajı

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    // Servisten yüklenme durumunu al
    this.isLoading$ = this.authService.isLoading$;
    this.initForm();

    // Kullanıcı zaten giriş yaptıysa ana sayfaya yönlendir
    if (this.authService.currentUserValue) {
      this.router.navigate(['/']);
    }
  }

  // Form kontrollerini oluştur
  initForm(): void {
    this.registerForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  // Form gönderildiğinde çalışır
  submit(): void {
    this.hasError = false;
    this.errorMessage = '';

    const { fullName, email, password } = this.registerForm.value;

    if (!fullName || !email || !password) {
      this.hasError = true;
      this.errorMessage = 'Tüm alanlar zorunludur.';
      return;
    }

    this.authService
      .register(fullName, email, password)
      .pipe(first())
      .subscribe({
        next: (user: UserModel | undefined) => {
          if (user) {
            this.router.navigate(['/']);
          } else {
            this.hasError = true;
            this.errorMessage = 'Kayıt başarısız.';
          }
        },
        error: (err) => {
          this.hasError = true;
          this.errorMessage = err?.error?.message || 'Sunucu hatası';
          console.error('Register error:', err);
        },
      });
  }
}

