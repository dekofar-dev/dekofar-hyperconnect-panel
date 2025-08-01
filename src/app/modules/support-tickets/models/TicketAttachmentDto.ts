// ticket-attachment.model.ts
export interface TicketAttachmentDto {
  id: number;
  fileName: string;
    url: string; // ✅ Dosyaya erişilecek tam link

  contentType: string;
  uploadedAt: string;
    status: string;       // "Açık", "İnceleme", vs. metinsel olabilir
  changedBy: string;    // kullanıcı adı veya ID
  changedAt: string;    // tarih
}