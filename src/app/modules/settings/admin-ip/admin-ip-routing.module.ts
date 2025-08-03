import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AllowedIpsComponent } from './allowed-ips.component';
import { AuthGuard } from '../../auth/services/auth.guard';
import { RoleGuard } from '../../auth/services/role.guard';

// Yetkili IP modülü için yönlendirme
const routes: Routes = [
  {
    path: '',
    component: AllowedIpsComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminIpRoutingModule {}
