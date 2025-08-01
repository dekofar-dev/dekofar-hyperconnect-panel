import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrderListComponent } from './components/order-list/order-list.component';
import { OrderDetailComponent } from './components/order-detail/order-detail.component';
import { RoleGuard } from '../auth/services/role.guard';

const routes: Routes = [
  {
    path: '',
    component: OrderListComponent,
    canActivate: [RoleGuard],
    data: { roles: ['Admin'] }
  },
  {
    path: 'detail/:id',
    component: OrderDetailComponent,
    canActivate: [RoleGuard],
    data: { roles: ['Admin'] }
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrdersRoutingModule {}
