import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, Params } from '@angular/router';
import { OrdersService } from '../../services/orders.service';
import { ShopifyOrderDetailDto } from '../../model/shopfy/shopify-order-detail.model';

@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss'],
})
export class OrderDetailComponent implements OnInit {
  order?: ShopifyOrderDetailDto;
  isLoading = true;
  hasError = false;

  updatedNote = '';
  updatedTags = '';
  customerType: 'standard' | 'elite' | 'problematic' = 'standard';
  defaultImage = 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTVYuzjheaiGfDZwUhUJnJQwfXTzJWDCJ90xw&s';

  allOrderIds: number[] = [];
  currentIndex: number = -1;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private ordersService: OrdersService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params: Params) => {
      const orderId = Number(params['id']);
      const storedIds = localStorage.getItem('orderIdList');
      if (storedIds) {
        this.allOrderIds = JSON.parse(storedIds);
        this.currentIndex = this.allOrderIds.indexOf(orderId);
      }

      if (!isNaN(orderId)) {
        this.fetchOrder(orderId);
      } else {
        this.hasError = true;
      }
    });
  }
// 🔽 Bu iki getter'ı class içine ekle
get hasPreviousOrder(): boolean {
  return this.currentIndex > 0;
}

get hasNextOrder(): boolean {
  return this.currentIndex < this.allOrderIds.length - 1;
}

  fetchOrder(orderId: number): void {
    this.isLoading = true;
    this.ordersService.getOrderDetail(orderId).subscribe({
      next: (res) => {
        res.lineItems.forEach((item) => {
          if (!item.imageUrl) {
            item.imageUrl = this.defaultImage;
          }
        });

        this.order = res;
        this.updatedNote = res.note || '';
        this.updatedTags = res.tags || '';
        this.isLoading = false;
      },
      error: (err) => {
        console.error('❌ Sipariş detayı alınamadı:', err);
        this.hasError = true;
        this.isLoading = false;
      },
    });
  }

  goToPreviousOrder(): void {
    if (this.currentIndex > 0) {
      const prevId = this.allOrderIds[this.currentIndex - 1];
      this.router.navigate(['/orders/detail', prevId]);
    }
  }

  goToNextOrder(): void {
    if (this.currentIndex < this.allOrderIds.length - 1) {
      const nextId = this.allOrderIds[this.currentIndex + 1];
      this.router.navigate(['/orders/detail', nextId]);
    }
  }

  copyToClipboard(value?: string | null): void {
    if (value) {
      navigator.clipboard.writeText(value).then(() => {
        console.log('📋 Kopyalandı:', value);
      });
    }
  }

  openWhatsApp(phone?: string): void {
    if (!phone) {
      alert('Telefon numarası bulunamadı.');
      return;
    }
    const sanitized = phone.replace(/\D/g, '');
    const message = encodeURIComponent('Merhaba, siparişiniz hakkında bilgi vermek istiyoruz.');
    window.open(`https://wa.me/${sanitized}?text=${message}`, '_blank');
  }

  onCancelOrder(): void {
    if (confirm('Siparişi iptal etmek istediğinize emin misiniz?')) {
      alert('❌ Sipariş iptal edildi (örnek işlem)');
    }
  }

  onMarkAsPaid(): void {
    alert('💳 Sipariş "ödendi" olarak işaretlendi (örnek işlem)');
  }

  onArchiveOrder(): void {
    alert('🗃️ Sipariş arşivlendi (örnek işlem)');
  }

  saveNote(): void {
    if (!this.order) return;
    this.ordersService
      .updateOrderNote(this.order.orderId, this.updatedNote)
      .subscribe({
        next: () => alert('Not kaydedildi'),
        error: () => alert('Not güncellenemedi')
      });
  }

  saveTags(): void {
    if (!this.order) return;
    this.ordersService
      .updateOrderTags(this.order.orderId, this.updatedTags)
      .subscribe({
        next: () => alert('Etiketler güncellendi'),
        error: () => alert('Etiketler güncellenemedi')
      });
  }

  sendInvoice(orderId: number): void {
    alert('📩 Fatura gönderme işlemi başlatıldı (örnek)');
  }

  onCreateSupportTicket(): void {
    if (!this.order) return;

    const random = Math.floor(Math.random() * 1000) + 1;
    const ticketCode = `TKT-${this.order.orderNumber}-${random}`;

    const customer = this.order.customer || {};
    const lineItem = this.order.lineItems[0] || {};

    const ticketData = {
      subject: ticketCode,
      category: 'Kargo Sorunu',
      description: '',
      shopifyOrderId: this.order.orderNumber,
      orderSummary: {
        orderNumber: this.order.orderNumber,
        productTitle: lineItem.title || 'Ürün bilgisi yok',
        quantity: lineItem.quantity || 1,
        totalPrice: `${this.order.totalPrice} ${this.order.currency}`,
        customerName: `${customer.firstName || ''} ${customer.lastName || ''}`.trim(),
        imageUrl: lineItem.imageUrl || this.defaultImage,
        address: this.order.billingAddress?.address1 || '-',
        shippingStatus: this.order.fulfillmentStatus === 'fulfilled' ? 'Teslim Edildi' : 'Hazırlanıyor',
      },
    };

    localStorage.setItem('pendingTicket', JSON.stringify(ticketData));
    this.router.navigate(['/support-tickets/create']);
  }

  shouldShowNavigation(): boolean {
    return this.order?.fulfillmentStatus !== 'fulfilled';
  }
  
}
