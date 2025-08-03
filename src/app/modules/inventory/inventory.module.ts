import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InventoryRoutingModule } from './inventory-routing.module';
import { ProductListComponent } from './components/product-list/product-list.component';
import { StockTrackingComponent } from './components/stock-tracking/stock-tracking.component';

/** Envanter işlemleri için modül */
@NgModule({
  declarations: [ProductListComponent, StockTrackingComponent],
  imports: [CommonModule, InventoryRoutingModule],
})
export class InventoryModule {}
