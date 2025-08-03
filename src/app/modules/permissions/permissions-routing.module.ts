import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PermissionManagementComponent } from './permission-management.component';
import { AuthGuard } from '../auth/services/auth.guard';
import { RoleGuard } from '../auth/services/role.guard';

// İzin modülü için yönlendirme
const routes: Routes = [
  {
    path: '',
    component: PermissionManagementComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PermissionsRoutingModule {}
