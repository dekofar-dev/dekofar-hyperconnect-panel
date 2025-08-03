import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProfileComponent } from './profile.component';
import { ChangePasswordComponent } from './components/change-password/change-password.component';
import { ProfileSettingsComponent } from './settings/profile-settings.component';

/** Profil modülünün yönlendirme ayarları */
const routes: Routes = [
  // Profil ana sayfası
  { path: '', component: ProfileComponent },
  // Şifre değiştirme sayfası
  { path: 'change-password', component: ChangePasswordComponent },
    { path: 'settings', component: ProfileSettingsComponent },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProfileRoutingModule {}
