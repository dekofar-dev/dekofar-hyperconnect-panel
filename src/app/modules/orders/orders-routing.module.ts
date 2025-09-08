import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrderListComponent } from './components/order-list/order-list.component';
import { OrderDetailComponent } from './components/order-detail/order-detail.component';
import { ManualOrderCreateComponent } from './components/manual-order-create/manual-order-create.component';
import { OrderItemsSummaryComponent } from './components/order-items-summary/order-items-summary.component';

const routes: Routes = [
  { path: '', component: OrderListComponent },
  { path: 'create', component: ManualOrderCreateComponent },
  { path: 'siparis-listesi', component: OrderItemsSummaryComponent }, // ← parametreli rotadan önce
  { path: ':id', component: OrderDetailComponent }, // en sona
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrdersRoutingModule {}
