import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/modules/auth';
import { SupportTicketService } from '../../services/support-ticket.service';
import { SupportTicketDto } from '../../models/support-ticket.model';

@Component({
  selector: 'app-ticket-detail',
  templateUrl: './ticket-detail.component.html',
})
export class TicketDetailComponent implements OnInit {
  ticketId!: string;
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
  selectedUserId: string | null = null;

  noteText = '';
  replyFiles: File[] = [];

  assignableUsers: { id: string; fullName: string }[] = [];

  isAdmin = false;
  isSupport = false;
  currentUserEmail: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private ticketService: SupportTicketService,
    private auth: AuthService
  ) {}

  ngOnInit(): void {
    const current = this.auth.getAuthFromLocalStorage();
    this.isAdmin = current?.role === 'Admin';
    this.isSupport = current?.role === 'Support';
    this.currentUserEmail = current?.email || null;

    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.ticketId = id;
        this.fetchTicketDetail();
      }
    });

    if (this.isAdmin || this.isSupport) {
      this.ticketService.getAssignableUsers().subscribe(users => {
        this.assignableUsers = users;
      });
    }
  }

  fetchTicketDetail(): void {
    this.loading = true;
    this.ticketService.getById(this.ticketId).subscribe({
      next: (res) => {
        this.ticket = res;
        this.selectedStatus = res.status;
        this.selectedPriority = res.priority;
        this.selectedUserId = res.assignedToUserId?.toString() || null;
        this.loading = false;

        this.ticketService.markAsRead(this.ticketId).subscribe();
      },
      error: (err) => {
        console.error('Detay çekme hatası:', err);
        this.errorMessage = 'Detaylar yüklenirken hata oluştu.';
        this.loading = false;
      }
    });
  }

  updateTicket(): void {
    if (!this.ticket) return;
    const data: any = {
      status: this.selectedStatus,
      priority: this.selectedPriority,
      assignedToUserId: this.selectedUserId
    };
    this.ticketService.update(this.ticket.id.toString(), data).subscribe({
      next: () => alert('Güncellendi'),
      error: () => alert('Güncellenemedi')
    });
  }

  markResolved(): void {
    if (!this.ticket) return;
    this.ticketService.markAsResolved(this.ticket.id.toString()).subscribe({
      next: () => this.fetchTicketDetail(),
      error: () => alert('Çözüm kaydedilemedi')
    });
  }

  sendReply(): void {
    if (!this.ticket || (!this.noteText.trim() && this.replyFiles.length === 0)) return;

    this.ticketService.reply(this.ticket.id.toString(), this.noteText, this.replyFiles).subscribe({
      next: () => {
        this.noteText = '';
        this.replyFiles = [];
        this.fetchTicketDetail();
      },
      error: () => alert('Yanıt gönderilemedi')
    });
  }

  onFilesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.replyFiles = Array.from(input.files);
    }
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
    return !!this.ticket?.attachments?.length;
  }

  hasNotes(): boolean {
    return !!this.ticket?.notes?.length;
  }

  hasLogs(): boolean {
    return !!this.ticket?.logs?.length;
  }

  hasHistory(): boolean {
    return !!this.ticket?.history?.length;
  }
}
