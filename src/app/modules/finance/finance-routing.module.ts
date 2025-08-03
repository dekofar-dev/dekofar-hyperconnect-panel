import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CodPaymentsComponent } from './components/cod-payments/cod-payments.component';
import { ShippingInvoicesComponent } from './components/shipping-invoices/shipping-invoices.component';
import { ExpensesComponent } from './components/expenses/expenses.component';

/** Finans modülünün yönlendirme ayarları */
const routes: Routes = [
  { path: 'cod-payments', component: CodPaymentsComponent },
  { path: 'shipping-invoices', component: ShippingInvoicesComponent },
  { path: 'expenses', component: ExpensesComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class FinanceRoutingModule {}
