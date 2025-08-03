import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileRoutingModule } from './profile-routing.module';
import { ProfileComponent } from './profile.component';
import { ChangePasswordComponent } from './components/change-password/change-password.component';

/** Profil işlemleri için modül */
@NgModule({
  declarations: [ProfileComponent, ChangePasswordComponent],
  imports: [CommonModule, ProfileRoutingModule],
})
export class ProfileModule {}
