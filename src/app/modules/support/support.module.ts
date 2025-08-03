import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SupportRoutingModule } from './support-routing.module';
import { PendingSupportComponent } from './components/pending-support/pending-support.component';
import { UndeliveredSupportComponent } from './components/undelivered-support/undelivered-support.component';
import { ReturnRequestsComponent } from './components/return-requests/return-requests.component';

/** Destek işlemleri için modül */
@NgModule({
  declarations: [
    PendingSupportComponent,
    UndeliveredSupportComponent,
    ReturnRequestsComponent,
  ],
  imports: [CommonModule, SupportRoutingModule],
})
export class SupportModule {}
