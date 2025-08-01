import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { SupportTicketService } from '../../services/support-ticket.service';
import { SupportTicketDto } from '../../models/support-ticket.model';
import { AuthService } from 'src/app/modules/auth';
import { SupportTicketQuery } from '../../models/support-ticket-query.model';

@Component({
  selector: 'app-ticket-list',
  templateUrl: './ticket-list.component.html'
})
export class TicketListComponent implements OnInit, OnDestroy {
  tickets: SupportTicketDto[] = [];
  loading = false;
  page = 1;
  pageSize = 10;
  hasMore = true;
  search = '';
  selectedCategory: number | null = null;
  selectedStatus: number | null = null;

  private subs: Subscription[] = [];

  constructor(
    private ticketService: SupportTicketService,
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTickets(true);
  }

  loadTickets(reset = false): void {
    if (reset) {
      this.page = 1;
      this.tickets = [];
      this.hasMore = true;
    }
    if (!this.hasMore || this.loading) return;

    this.loading = true;
    const currentUser = this.auth.getAuthFromLocalStorage();
    const isAdmin = currentUser?.role === 'Admin';

    const query: SupportTicketQuery = {
      pageNumber: this.page,
      pageSize: this.pageSize,
      category: this.selectedCategory ?? undefined,
      status: this.selectedStatus ?? undefined,
      search: this.search || undefined
    };

    const sub = this.ticketService.list(query).subscribe({
      next: (res) => {
        let items = res.items;
        if (!isAdmin) {
          items = items.filter(t => t.assignedToUserId === currentUser?.id);
        }
        this.tickets.push(...items);
        this.hasMore = items.length === this.pageSize;
        this.page++;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });

    this.subs.push(sub);
  }

  onSearchChange(): void {
    this.loadTickets(true);
  }

  filterByCategory(category: number | null): void {
    this.selectedCategory = category;
    this.loadTickets(true);
  }

  onStatusChange(status: number | null): void {
    this.selectedStatus = status;
    this.loadTickets(true);
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
  }

  // Atanan kişinin veya oluşturucunun baş harfleri
  getInitials(name?: string): string {
    if (!name) return '?';
    const parts = name.trim().split(' ');
    if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
    return parts[0].charAt(0).toUpperCase() + parts[1].charAt(0).toUpperCase();
  }

  // Kategori adı
  getCategoryName(category: number): string {
    const categories: { [key: number]: string } = {
      1: 'İade',
      2: 'Değişim',
      3: 'Eksik Ürün',
      4: 'Kargo Sorunu',
      5: 'Garanti',
      6: 'Fatura',
      7: 'Arama Talebi',
      99: 'Diğer'
    };
    return categories[category] || 'Bilinmeyen';
  }

  // Öncelik etiketi ve rengi
  getPriorityInfo(priority: number | null | undefined): { label: string; class: string } {
    switch (priority) {
      case 1: return { label: 'Yüksek', class: 'text-danger fw-bold' };
      case 2: return { label: 'Orta', class: 'text-warning fw-bold' };
      case 3: return { label: 'Düşük', class: 'text-success fw-bold' };
      default: return { label: '-', class: 'text-muted' };
    }
  }

  // Durum etiketi ve rengi
  getStatusInfo(ticket: SupportTicketDto): { label: string; class: string } {
    switch (ticket.status) {
      case 1: return { label: 'Açık', class: 'badge bg-primary' };
      case 2: return { label: 'İşlemde', class: 'badge bg-warning text-dark' };
      case 3: return { label: 'Çözüldü', class: 'badge bg-success' };
      default: return { label: 'Bilinmiyor', class: 'badge bg-secondary' };
    }
  }

  // Etiketleri virgüle göre ayır
  getTagsArray(tags?: string): string[] {
    return tags ? tags.split(',').map(tag => tag.trim()) : [];
  }

  // deprecated local filtering methods removed
}
