import { Component, OnInit } from '@angular/core';
import { OrdersService } from './orders.service';
import { OrderSummary } from './order.model';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-orders-page',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {
  activeTab: 'shopify' | 'manual' = 'shopify';
  shopifyOrders: OrderSummary[] = [];
  manualOrders: OrderSummary[] = [];
  loading = false;

  constructor(
    private service: OrdersService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(params => {
      const src = params.get('source') as 'shopify' | 'manual';
      if (src) {
        this.activeTab = src;
      }
      this.loadOrders();
    });
  }

  selectTab(tab: 'shopify' | 'manual'): void {
    this.activeTab = tab;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { source: tab }
    });
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;
    const obs = this.activeTab === 'shopify'
      ? this.service.getShopifyOrders()
      : this.service.getManualOrders();
    obs.subscribe({
      next: orders => {
        if (this.activeTab === 'shopify') {
          this.shopifyOrders = orders;
        } else {
          this.manualOrders = orders;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  get orders(): OrderSummary[] {
    return this.activeTab === 'shopify' ? this.shopifyOrders : this.manualOrders;
  }

  createTicket(order: OrderSummary): void {
    const ticketData = {
      subject: `ORDER-${order.id}`,
      shopifyOrderId: order.source === 'shopify' ? String(order.id) : undefined,
      orderSummary: {
        orderNumber: order.id,
        totalPrice: order.total,
        customerName: order.customerName
      }
    };
    localStorage.setItem('pendingTicket', JSON.stringify(ticketData));
    this.router.navigate(['/support-tickets/create']);
  }
}
