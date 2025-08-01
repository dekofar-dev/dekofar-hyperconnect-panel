import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MailboxRoutingModule } from './mailbox-routing.module';
import { InboxComponent } from './components/inbox/inbox.component';
import { PendingRepliesComponent } from './components/pending-replies/pending-replies.component';
import { MessageDetailComponent } from './components/message-detail/message-detail.component';

@NgModule({
  declarations: [
    InboxComponent,
    PendingRepliesComponent,
    MessageDetailComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    MailboxRoutingModule
  ]
})
export class MailboxModule { }
