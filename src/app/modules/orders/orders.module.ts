import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms'; // ✅ EKLE
import { RouterModule } from '@angular/router';

import { OrderListComponent } from './components/order-list/order-list.component';
import { OrderDetailComponent } from './components/order-detail/order-detail.component';
import { ManualOrderCreateComponent } from './components/manual-order-create/manual-order-create.component'; // ✅ varsa
import { OrdersRoutingModule } from './orders-routing.module';
import { OrderItemsSummaryComponent } from './components/order-items-summary/order-items-summary.component';

@NgModule({
  declarations: [
    OrderListComponent,
    OrderDetailComponent,
    ManualOrderCreateComponent,
    OrderItemsSummaryComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule, // ✅ EKLE
    FormsModule,         // (opsiyonel ama önerilir)
    RouterModule,
    OrdersRoutingModule
  ]
})
export class OrdersModule {}
