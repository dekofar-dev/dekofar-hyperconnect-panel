import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SupportCategoriesComponent } from './support-categories.component';
import { AuthGuard } from '../auth/services/auth.guard';
import { RoleGuard } from '../auth/services/role.guard';

const routes: Routes = [
  {
    path: '',
    component: SupportCategoriesComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SupportCategoriesRoutingModule {}
