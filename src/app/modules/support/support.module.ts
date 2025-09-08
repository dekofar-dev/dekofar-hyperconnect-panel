import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SupportRoutingModule } from './support-routing.module';
import { PendingSupportComponent } from './components/pending-support/pending-support.component';
import { UndeliveredSupportComponent } from './components/undelivered-support/undelivered-support.component';
import { ReturnRequestsComponent } from './components/return-requests/return-requests.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { SmsInboxComponent } from './components/sms-inbox/sms-inbox.component';

/** Destek işlemleri için modül */
@NgModule({
  declarations: [
    PendingSupportComponent,
    UndeliveredSupportComponent,
    ReturnRequestsComponent,
    SmsInboxComponent, // <-- EKLENDİ
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    SupportRoutingModule
  ],
})
export class SupportModule {}
