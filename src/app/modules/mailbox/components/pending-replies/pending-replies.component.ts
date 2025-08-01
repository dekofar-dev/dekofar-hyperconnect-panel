import { Component, OnInit } from '@angular/core';
import { MessageService } from '../../services/message.service';
import { Message } from '../../models/message.model';

@Component({
  selector: 'app-mailbox-pending-replies',
  templateUrl: './pending-replies.component.html',
  styleUrls: ['./pending-replies.component.scss']
})
export class PendingRepliesComponent implements OnInit {
  messages: Message[] = [];

  constructor(private messageService: MessageService) {}

  ngOnInit(): void {
    this.messages = this.messageService.getPendingReplies();
  }
}
