import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router'; // <== Gerekli!
import { FormsModule } from '@angular/forms';   // <== ngModel varsa bu da gerekli!

import { OrderListComponent } from './components/order-list/order-list.component';
import { OrderDetailComponent } from './components/order-detail/order-detail.component';
import { OrdersRoutingModule } from './orders-routing.module';

@NgModule({
  declarations: [
    OrderListComponent,
    OrderDetailComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,        // ✅ routerLink, routerLinkActive için
    OrdersRoutingModule  // ✅ kendi routing modülün
  ]
})
export class OrdersModule {}
