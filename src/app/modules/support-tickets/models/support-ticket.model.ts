import { TicketNoteDto } from './ticket-note.model';
import { TicketLogDto } from './ticket-log.model';
import { TicketAttachmentDto } from './TicketAttachmentDto';
import { SupportTicketHistoryDto } from './ticket-history.model';

export enum SupportCategory {
  Iade = 1,
  Degisim = 2,
  EksikUrun = 3,
  KargoSorunu = 4,
  Garanti = 5,
  Fatura = 6,
  AramaTalebi = 7,
  Diger = 99
}

export enum SupportPriority {
  Yuksek = 1,
  Orta = 2,
  Dusuk = 3
}

export enum SupportStatus {
  Acik = 0,
  Inceleme = 1,
  CevapBekleniyor = 2,
  Kapandi = 3
}

// ✅ FORM GÖNDERİMİ İÇİN KULLANILIR
export interface SupportTicketCreateDto {
  subject: string;
  description: string;
  category: number;
  priority?: number;
  tags?: string;
  customerName: string;
  customerPhone: string;
  customerEmail?: string;
  shopifyOrderId?: string;
  dueDate?: string;

  // ✅ Atama yapılacaksa bu alan gerekli:
  assignedToUserId?: string;

  // Opsiyonel gösterim için
  assignedToUser?: {
    fullName: string;
  };
}

// ✅ API'DEN GELEN VERİ İÇİN KULLANILIR
export interface SupportTicketDto {
  id: number;
  ticketNumber: string;
  subject: string;
  description: string;
  category: SupportCategory;
  priority: SupportPriority;
  tags?: string;
  dueDate?: string;

  customerPhone?: string;
  customerEmail?: string;
  customerName?: string;
  shopifyOrderId?: string;

  assignedToUserId?: string;
  assignedToUser?: {
    fullName: string;
  };

  createdBy: string;
  createdAt: string;
  status: SupportStatus;

  notes: TicketNoteDto[];
  logs: TicketLogDto[];
  attachments: TicketAttachmentDto[];
  history: SupportTicketHistoryDto[];
}
