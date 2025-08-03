import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PendingReturnsComponent } from './components/pending-returns/pending-returns.component';
import { ApprovedReturnsComponent } from './components/approved-returns/approved-returns.component';
import { RejectedReturnsComponent } from './components/rejected-returns/rejected-returns.component';

/** İade modülünün yönlendirme ayarları */
const routes: Routes = [
  { path: 'pending', component: PendingReturnsComponent },
  { path: 'approved', component: ApprovedReturnsComponent },
  { path: 'rejected', component: RejectedReturnsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReturnsRoutingModule {}
