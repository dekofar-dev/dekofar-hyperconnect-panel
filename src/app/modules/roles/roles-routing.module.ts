import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoleManagementComponent } from './role-management.component';
import { AuthGuard } from '../auth/services/auth.guard';
import { RoleGuard } from '../auth/services/role.guard';

// Rol modülü için yönlendirmeler
const routes: Routes = [
  {
    path: '',
    component: RoleManagementComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class RolesRoutingModule {}
