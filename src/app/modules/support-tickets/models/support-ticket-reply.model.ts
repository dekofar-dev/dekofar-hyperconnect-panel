export interface SupportTicketReplyDto {
  id: string;
  ticketId: string;
  userId: string;
  fullName: string;         // ApplicationUser.FullName
  avatarUrl?: string;       // ApplicationUser.AvatarUrl
  message: string;
  attachmentUrl?: string;
  createdAt: string;        // ISO datetime string
}
