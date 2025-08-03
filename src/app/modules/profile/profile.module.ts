import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileRoutingModule } from './profile-routing.module';
import { ProfileComponent } from './profile.component';
import { ChangePasswordComponent } from './components/change-password/change-password.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProfileSettingsComponent } from './settings/profile-settings.component';

/** Profil işlemleri için modül */
@NgModule({
  declarations: [ProfileComponent, ChangePasswordComponent, ProfileSettingsComponent],
  imports: [CommonModule, ProfileRoutingModule,
    ReactiveFormsModule,
    FormsModule          


  ],
})
export class ProfileModule {}
