import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TicketListComponent } from './components/ticket-list/ticket-list.component';
import { TicketCreateComponent } from './components/ticket-create/ticket-create.component';
import { TicketDetailComponent } from './components/ticket-detail/ticket-detail.component';
import { AuthGuard } from '../auth/services/auth.guard';
import { RoleGuard } from '../auth/services/role.guard';

const routes: Routes = [
  {
    path: 'my',
    component: TicketListComponent,
    canActivate: [AuthGuard],
    data: { mode: 'my' }
  },
  {
    path: 'all',
    component: TicketListComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'], mode: 'all' }
  },
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
  {
    path: '',
    redirectTo: 'my',
    pathMatch: 'full'
  }
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SupportTicketsRoutingModule { }
