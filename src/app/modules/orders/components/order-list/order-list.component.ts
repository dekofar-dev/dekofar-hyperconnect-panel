import { Component, OnInit } from '@angular/core';
import { OrdersService } from '../../services/orders.service';
import { Order } from '../../model/shopfy/order.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order-list',
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.scss'],
})
export class OrderListComponent implements OnInit {
  confirmedOrders: Order[] = [];
  nextPageInfo: string | null = null;
  isLoading: boolean = false;
  hasError: boolean = false;

  // 🔍 Arama özellikleri
  searchQuery: string = '';
  searchResults: Order[] | null = null;
  searchTimeout: any = null;

  constructor(private shopifyService: OrdersService, private router: Router) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  // ✅ Normal siparişleri yükler (sayfalı)
  loadOrders(): void {
    this.isLoading = true;
    this.hasError = false;

    this.shopifyService.getConfirmedOrdersPaged(10, this.nextPageInfo || undefined).subscribe({
      next: (res) => {
        console.log('📦 Shopify Siparişleri:', res.items);
        this.confirmedOrders.push(...res.items);
        this.nextPageInfo = res.nextPageInfo || null;

        // 🧭 Navigasyon için ID listesi
        const orderIds = this.confirmedOrders.map((o) => o.id);
        localStorage.setItem('orderIdList', JSON.stringify(orderIds));

        this.isLoading = false;
      },
      error: (err) => {
        console.error('❌ Siparişler alınamadı:', err);
        this.hasError = true;
        this.isLoading = false;
      },
    });
  }

  // ✅ Sayfalı olarak daha fazla sipariş yükle
  loadMore(): void {
    if (this.searchResults) return; // 🔍 Arama açıkken sayfa yüklenmez
    if (this.nextPageInfo && !this.isLoading) {
      this.loadOrders();
    }
  }

  // ✅ Satıra tıklayınca detay sayfasına git
  goToOrder(orderId: number | string): void {
    this.router.navigate(['/orders/detail', orderId]);
  }

  // 🔍 Canlı arama input değiştiğinde çalışır
  onSearchInputChange(): void {
    clearTimeout(this.searchTimeout);

    if (!this.searchQuery.trim()) {
      // Arama kutusu temizlendi → normal listeye dön
      this.searchResults = null;
      this.confirmedOrders = [];
      this.nextPageInfo = null;
      this.loadOrders();
      return;
    }

    this.searchTimeout = setTimeout(() => {
      this.shopifyService.searchOrders(this.searchQuery).subscribe({
        next: (results) => {
          this.searchResults = results;

          // Navigasyon listesi güncelle
          const orderIds = results.map((o) => o.id);
          localStorage.setItem('orderIdList', JSON.stringify(orderIds));
        },
        error: () => {
          this.searchResults = [];
        },
      });
    }, 400); // Debounce
  }
}
