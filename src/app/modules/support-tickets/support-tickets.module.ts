import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SupportTicketsRoutingModule } from './support-tickets-routing.module';
import { SupportTicketsComponent } from './support-tickets.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TicketListComponent } from './components/ticket-list/ticket-list.component';
import { TicketCreateComponent } from './components/ticket-create/ticket-create.component';
import { TicketDetailComponent } from './components/ticket-detail/ticket-detail.component';
import { ModalsModule } from 'src/app/_metronic/partials';

@NgModule({
  declarations: [
    SupportTicketsComponent,
    TicketListComponent,
    TicketCreateComponent,
    TicketDetailComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ModalsModule,
    ReactiveFormsModule,
    SupportTicketsRoutingModule
  ]
})
export class SupportTicketsModule { }
