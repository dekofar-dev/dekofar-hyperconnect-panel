import { Injectable } from '@angular/core';
import { Message } from '../models/message.model';

@Injectable({ providedIn: 'root' })
export class MessageService {
  private messages: Message[] = [
    {
      id: 1,
      channel: 'WhatsApp',
      senderName: 'Ahmet Yılmaz',
      message: 'Hi, I’d like to ask about the product.',
      timestamp: '2025-08-01T13:15:00Z',
      status: 'Pending Reply'
    },
    {
      id: 2,
      channel: 'Instagram',
      senderName: 'elif.celik',
      message: 'When will my order arrive?',
      timestamp: '2025-08-01T12:50:00Z',
      status: 'Answered'
    }
  ];

  getMessages(): Message[] {
    return this.messages;
  }

  getPendingReplies(): Message[] {
    return this.messages.filter(m => m.status === 'Pending Reply');
  }
}
