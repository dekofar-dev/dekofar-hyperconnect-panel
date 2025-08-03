import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReturnsRoutingModule } from './returns-routing.module';
import { PendingReturnsComponent } from './components/pending-returns/pending-returns.component';
import { ApprovedReturnsComponent } from './components/approved-returns/approved-returns.component';
import { RejectedReturnsComponent } from './components/rejected-returns/rejected-returns.component';

/** İade işlemleri için modül */
@NgModule({
  declarations: [
    PendingReturnsComponent,
    ApprovedReturnsComponent,
    RejectedReturnsComponent,
  ],
  imports: [CommonModule, ReturnsRoutingModule],
})
export class ReturnsModule {}
