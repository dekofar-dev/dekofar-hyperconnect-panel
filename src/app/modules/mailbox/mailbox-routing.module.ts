import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InboxComponent } from './components/inbox/inbox.component';
import { PendingRepliesComponent } from './components/pending-replies/pending-replies.component';

const routes: Routes = [
  { path: 'inbox', component: InboxComponent },
  { path: 'pending-replies', component: PendingRepliesComponent },
  { path: '', redirectTo: 'inbox', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MailboxRoutingModule { }
