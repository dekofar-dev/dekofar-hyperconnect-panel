import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SupportTicketService } from '../../services/support-ticket.service';
import { SupportTicketDto } from '../../models/support-ticket.model';

@Component({
  selector: 'app-ticket-detail',
  templateUrl: './ticket-detail.component.html',
})
export class TicketDetailComponent implements OnInit {
  ticketId!: number;
  ticket?: SupportTicketDto;
  loading = true;
  errorMessage: string | null = null;
  statusOptions = [
    { id: 0, label: 'Açık' },
    { id: 1, label: 'İnceleme' },
    { id: 2, label: 'Cevap Bekleniyor' },
    { id: 3, label: 'Kapandı' }
  ];
  priorityOptions = [
    { id: 1, label: 'Yüksek' },
    { id: 2, label: 'Orta' },
    { id: 3, label: 'Düşük' }
  ];
  selectedStatus: number | null = null;
  selectedPriority: number | null = null;
  noteText = '';

  constructor(
    private route: ActivatedRoute,
    private ticketService: SupportTicketService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.ticketId = +id;
        this.fetchTicketDetail();
      }
    });
  }

  fetchTicketDetail(): void {
    this.loading = true;
    this.ticketService.getById(this.ticketId).subscribe({
      next: (res) => {
        this.ticket = res;
        this.selectedStatus = res.status;
        this.selectedPriority = res.priority;
        this.loading = false;
      },
      error: (err) => {
        console.error('Detay çekme hatası:', err);
        this.errorMessage = 'Detaylar yüklenirken hata oluştu.';
        this.loading = false;
      }
    });
  }

  getCategoryName(category: number): string {
    const categories: { [key: number]: string } = {
      0: 'İade',
      1: 'Değişim',
      2: 'Eksik Ürün',
      3: 'Kargo Sorunu',
      4: 'Ödeme Sorunu',
      5: 'Genel Bilgi',
      6: 'Diğer',
      7: 'Garanti'
    };
    return categories[category] || 'Bilinmeyen';
  }

  getPriorityLabel(priority: number): string {
    switch (priority) {
      case 1: return 'Yüksek';
      case 2: return 'Orta';
      case 3: return 'Düşük';
      default: return '-';
    }
  }

  getStatusLabel(status: number): string {
    switch (status) {
      case 0: return 'Bekliyor';
      case 1: return 'Atandı';
      case 2: return 'Devam Ediyor';
      case 3: return 'Çözüldü';
      case 4: return 'İptal Edildi';
      default: return '-';
    }
  }

  getTicketType(): string {
    return this.ticket?.shopifyOrderId ? 'Shopify Referanslı' : 'Manuel';
  }

  formatDate(dateString: string | undefined | null): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleString('tr-TR');
  }

  hasAttachments(): boolean {
    return !!this.ticket?.attachments && this.ticket.attachments.length > 0;
  }

  hasNotes(): boolean {
    return !!this.ticket?.notes && this.ticket.notes.length > 0;
  }

  hasLogs(): boolean {
    return !!this.ticket?.logs && this.ticket.logs.length > 0;
  }

  hasHistory(): boolean {
    return !!this.ticket?.history && this.ticket.history.length > 0;
  }

  updateTicket(): void {
    if (!this.ticket) return;
    const data: any = {
      status: this.selectedStatus,
      priority: this.selectedPriority
    };
    this.ticketService.update(this.ticket.id, data).subscribe({
      next: () => alert('Güncellendi'),
      error: () => alert('Güncellenemedi')
    });
  }

  addNote(): void {
    if (!this.ticket || !this.noteText.trim()) return;
    this.ticketService.addNote(this.ticket.id, this.noteText).subscribe({
      next: () => {
        this.noteText = '';
        this.fetchTicketDetail();
      },
      error: () => alert('Not eklenemedi')
    });
  }
}
