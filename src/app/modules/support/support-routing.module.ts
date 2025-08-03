import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PendingSupportComponent } from './components/pending-support/pending-support.component';
import { UndeliveredSupportComponent } from './components/undelivered-support/undelivered-support.component';
import { ReturnRequestsComponent } from './components/return-requests/return-requests.component';

/** Destek modülünün yönlendirme ayarları */
const routes: Routes = [
  { path: 'pending-support', component: PendingSupportComponent },
  { path: 'undelivered', component: UndeliveredSupportComponent },
  { path: 'return-requests', component: ReturnRequestsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SupportRoutingModule {}
