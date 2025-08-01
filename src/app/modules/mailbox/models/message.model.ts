export interface Message {
  id: number;
  channel: string;
  senderName: string;
  message: string;
  timestamp: string;
  status: 'Pending Reply' | 'Answered' | 'Spam';
}
