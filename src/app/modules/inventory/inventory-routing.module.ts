import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductListComponent } from './components/product-list/product-list.component';
import { StockTrackingComponent } from './components/stock-tracking/stock-tracking.component';

/** Envanter modülünün yönlendirme ayarları */
const routes: Routes = [
  { path: 'product-list', component: ProductListComponent },
  { path: 'stock-tracking', component: StockTrackingComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class InventoryRoutingModule {}
