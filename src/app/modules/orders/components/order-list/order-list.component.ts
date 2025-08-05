import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { OrderModel } from '../../models/order.model';

@Component({
  selector: 'app-order-list',
  templateUrl: './order-list.component.html',
})
export class OrderListComponent implements OnInit {
  orders: OrderModel[] = [];
  filteredOrders: OrderModel[] = [];
  currentFilter: string = 'all';
  searchQuery: string = '';
  isLoading = false;
  hasError = false;

  nextPageInfo?: string;
  previousPageInfo?: string;
  currentPageInfo?: string;
  searchResults: boolean = false;

  readonly pageSize: number = 20;

  constructor(private orderService: OrderService, private router: Router) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(pageInfo?: string, direction: 'next' | 'prev' = 'next'): void {
    this.isLoading = true;
    this.hasError = false;

    this.orderService.getCombinedOrders(this.pageSize, pageInfo, direction).subscribe({
      next: (res) => {
        this.orders = res.items;
        this.filteredOrders = [...this.orders];
        this.nextPageInfo = res.nextPageInfo ?? undefined;
        this.previousPageInfo = res.previousPageInfo ?? undefined;
        this.currentPageInfo = pageInfo;
        this.searchResults = false;
        this.isLoading = false;
      },
      error: () => {
        this.hasError = true;
        this.isLoading = false;
      },
    });
  }

  loadNextPage(): void {
    if (this.nextPageInfo) {
      this.loadOrders(this.nextPageInfo, 'next');
    }
  }

  loadPreviousPage(): void {
    if (this.previousPageInfo) {
      this.loadOrders(this.previousPageInfo, 'prev');
    }
  }

  onSearchInputChange(): void {
    const query = this.searchQuery.trim();
    if (!query) {
      this.filteredOrders = [...this.orders];
      this.searchResults = false;
      return;
    }

    this.isLoading = true;
    this.orderService.searchOrders(query).subscribe({
      next: (results) => {
        this.filteredOrders = results;
        this.searchResults = true;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Arama hatası', err);
        this.filteredOrders = [];
        this.searchResults = true;
        this.isLoading = false;
      }
    });
  }

  applyFilter(): void {
    if (this.currentFilter === 'all') {
      this.filteredOrders = [...this.orders];
    } else if (this.currentFilter === 'manual') {
      this.filteredOrders = this.orders.filter(
        (order) => order.source === 'Manual'
      );
    } else {
      this.filteredOrders = this.orders.filter(
        (order) => order.status?.toLowerCase() === this.currentFilter.toLowerCase()
      );
    }
  }

  getShopifyBadgeClass(order: OrderModel): string {
    const status = (order.status || '').toLowerCase();
    switch (status) {
      case 'tamamlandı': return 'badge-light-success';
      case 'hazırlanıyor': return 'badge-light-warning';
      case 'beklemede': return 'badge-light-primary';
      case 'iptal': return 'badge-light-dark';
      case 'iade edildi': return 'badge-light-danger';
      case 'kısmi iade': return 'badge-light-info';
      case 'onaylandı': return 'badge-light-purple';
      default: return 'badge-light';
    }
  }

  getAvatarBgClass(order: OrderModel): string {
    const status = (order.status || '').toLowerCase();
    switch (status) {
      case 'tamamlandı': return 'bg-success';
      case 'hazırlanıyor': return 'bg-warning';
      case 'beklemede': return 'bg-primary';
      case 'iptal': return 'bg-dark';
      case 'iade edildi': return 'bg-danger';
      case 'kısmi iade': return 'bg-info';
      case 'onaylandı': return 'bg-purple';
      default: return 'bg-secondary';
    }
  }

  getFormattedDate(dateStr: string): string {
    const date = new Date(dateStr);
    const now = new Date();

    const isToday = date.toDateString() === now.toDateString();
    const yesterday = new Date();
    yesterday.setDate(now.getDate() - 1);
    const isYesterday = date.toDateString() === yesterday.toDateString();

    const diffInDays = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60 * 24));
    const saatDakika = date.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });

    if (isToday) return `Bugün saat ${saatDakika}`;
    if (isYesterday) return `Dün saat ${saatDakika}`;
    if (diffInDays < 7) {
      const gunAdi = date.toLocaleDateString('tr-TR', { weekday: 'long' });
      return `${gunAdi.charAt(0).toUpperCase() + gunAdi.slice(1)} saat ${saatDakika}`;
    }
    return `${date.toLocaleDateString('tr-TR')} saat ${saatDakika}`;
  }

  goToOrder(id: string | number): void {
    this.router.navigate(['/orders', id.toString()]);
  }

  loadMore(): void {
    this.loadNextPage();
  }

  getSourceLabel(order: OrderModel): string {
    return order.source === 'Manual' ? 'Manuel' : 'Shopify';
  }

  getBadgeClass(order: OrderModel): string {
    return order.source === 'Shopify'
      ? this.getShopifyBadgeClass(order)
      : this.getManualBadgeClass(order);
  }

  getManualBadgeClass(order: OrderModel): string {
    const status = (order.status || '').toLowerCase();
    switch (status) {
      case 'pending':
      case 'beklemede': return 'badge-light-warning';
      case 'completed':
      case 'tamamlandı': return 'badge-light-success';
      case 'cancelled':
      case 'iptal': return 'badge-light-danger';
      default: return 'badge-light';
    }
  }
}
