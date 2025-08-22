import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ShopifyExportComponent } from './components/shopify-export/shopify-export.component';
import { OrderContactExportComponent } from './components/order-contact-export/order-contact-export.component';
import { ExistingCustomerOrdersComponent } from './existing-customer-orders/existing-customer-orders.component';

const routes: Routes = [
  { path: 'shopify', component: ShopifyExportComponent },
  { path: 'contacts', component: OrderContactExportComponent },
  { path: 'existing-customers', component: ExistingCustomerOrdersComponent },
  { path: '', redirectTo: 'shopify', pathMatch: 'full' }, // default
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ExcelToolsRoutingModule {}
