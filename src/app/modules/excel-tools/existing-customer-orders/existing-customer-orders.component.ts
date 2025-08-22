import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../orders/services/order.service';
import { OrderModel } from '../../orders/models/order.model';
import { CUSTOMER_PHONE_SET } from '../data/customer-phones';

@Component({
  selector: 'app-existing-customer-orders',
  templateUrl: './existing-customer-orders.component.html'
})
export class ExistingCustomerOrdersComponent implements OnInit {
  matchedOrders: OrderModel[] = [];
  totalMatches = 0;

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    const sevenDaysAgo = new Date();
    sevenDaysAgo.setDate(sevenDaysAgo.getDate() - 7);

    this.orderService.getShopifyOrders().subscribe(orders => {
      // 1. Son 7 gün siparişleri filtrele
      const last7Days = orders.filter(o => new Date(o.createdAt) >= sevenDaysAgo);

      // 2. Telefon eşleşmesi yap
      this.matchedOrders = last7Days.filter(o =>
        CUSTOMER_PHONE_SET.has(this.normalizePhone(o.phone))
      );

      this.totalMatches = this.matchedOrders.length;
    });
  }

private normalizePhone(phone?: string): string {
  if (!phone) return '';
  return (phone ?? '')
    .replace(/\D/g, '')
    .replace(/^90/, '') // +90 veya 90 varsa sil
    .replace(/^0/, '')  // baştaki 0 varsa sil
    .slice(-10);        // son 10 haneyi al
}

}
