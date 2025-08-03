import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UsersService } from '../services/users.service';
import { AuthService } from '../../auth/services/auth.service';
import { UserModel } from '../../auth/models/user.model';

// Profil ayarları için component
@Component({
  selector: 'app-profile-settings',
  templateUrl: './profile-settings.component.html',
})
export class ProfileSettingsComponent implements OnInit {
  // Profil bilgileri formu
  profileForm: FormGroup;
  // Şifre değişim formu
  passwordForm: FormGroup;
  // PIN ayarlama formu
  pinForm: FormGroup;
  // PIN doğrulama formu
  verifyPinForm: FormGroup;
  // Avatar önizleme adresi
  avatarPreview: string | null = null;
  // Kullanıcı kimliği
  userId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private usersService: UsersService,
    private authService: AuthService
  ) {
    // Form gruplarını oluştur
    this.profileForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
    });

    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', Validators.required],
    });

    this.pinForm = this.fb.group({
      pin: ['', [Validators.required, Validators.pattern(/^[0-9]{4}$/)]],
    });

    this.verifyPinForm = this.fb.group({
      pin: ['', [Validators.required, Validators.pattern(/^[0-9]{4}$/)]],
    });
  }

  ngOnInit(): void {
    // Mevcut kullanıcı bilgisini yükle
    this.usersService.getMe().subscribe((user: UserModel) => {
      this.userId = user.id;
      // Gelen kullanıcı resmini önizlemeye ekle
      this.avatarPreview = user.pic ?? null;
      this.profileForm.patchValue({
        fullName: user.fullName,
        email: user.email,
        phone: user.phone,
      });
    });
  }

  // Profil bilgilerini sunucuya kaydet
  saveProfile(): void {
    if (this.profileForm.invalid) return;
    this.usersService.updateMe(this.profileForm.value).subscribe(() => {
      alert('Profil güncellendi');
      this.authService.getProfile().subscribe();
    });
  }

  // Şifreyi sunucuya göndererek değiştir
  changePassword(): void {
    if (this.passwordForm.invalid) return;
    const { currentPassword, newPassword } = this.passwordForm.value;
    this.usersService
      .changePassword(currentPassword, newPassword)
      .subscribe(() => {
        alert('Şifre değiştirildi');
        this.passwordForm.reset();
      });
  }

  // Yeni avatar seçildiğinde yükle ve önizlemeyi güncelle
  onAvatarSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files && input.files[0];
    if (!file || !this.userId) return;

    const reader = new FileReader();
    reader.onload = () => (this.avatarPreview = reader.result as string);
    reader.readAsDataURL(file);

    this.usersService.uploadAvatar(this.userId, file).subscribe((res) => {
      this.avatarPreview = res.avatarUrl;
      this.authService.getProfile().subscribe();
    });
  }

  // Avatarı kaldır
  removeAvatar(): void {
    if (!this.userId) return;
    this.usersService.removeAvatar(this.userId).subscribe(() => {
      this.avatarPreview = null;
      this.authService.getProfile().subscribe();
    });
  }

  // PIN ayarla
  setPin(): void {
    if (this.pinForm.invalid) return;
    this.usersService.setPin(this.pinForm.value.pin).subscribe(() => {
      alert('PIN ayarlandı');
      this.pinForm.reset();
    });
  }

  // PIN doğrula
  verifyPin(): void {
    if (this.verifyPinForm.invalid) return;
    this.usersService.verifyPin(this.verifyPinForm.value.pin).subscribe(() => {
      alert('PIN doğrulandı');
      this.verifyPinForm.reset();
    });
  }
}
