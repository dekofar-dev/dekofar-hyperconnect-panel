import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceRoutingModule } from './finance-routing.module';
import { CodPaymentsComponent } from './components/cod-payments/cod-payments.component';
import { ShippingInvoicesComponent } from './components/shipping-invoices/shipping-invoices.component';
import { ExpensesComponent } from './components/expenses/expenses.component';

/** Finans işlemleri için modül */
@NgModule({
  declarations: [
    CodPaymentsComponent,
    ShippingInvoicesComponent,
    ExpensesComponent,
  ],
  imports: [CommonModule, FinanceRoutingModule],
})
export class FinanceModule {}
