import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SupportTicketsComponent } from './support-tickets.component';
import { TicketListComponent } from './components/ticket-list/ticket-list.component';
import { TicketCreateComponent } from './components/ticket-create/ticket-create.component';
import { TicketDetailComponent } from './components/ticket-detail/ticket-detail.component';
import { AuthGuard } from '../auth/services/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: TicketListComponent,
    canActivate: [AuthGuard]
  },           // /support-tickets
  {
    path: 'create',
    component: TicketCreateComponent,
    canActivate: [AuthGuard]
  },   // /support-tickets/create
  {
    path: ':id',
    component: TicketDetailComponent,
    canActivate: [AuthGuard]
  },      // /support-tickets/:id
  
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SupportTicketsRoutingModule { }
