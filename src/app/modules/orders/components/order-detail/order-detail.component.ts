import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { OrderModel } from '../../models/order.model';

@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss']
})
export class OrderDetailComponent implements OnInit {
  order: OrderModel | null = null;
  isLoading = true;
  errorMessage: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    const source = this.route.snapshot.queryParamMap.get('source'); // Shopify veya Manual

    if (id && source) {
      this.loadOrderDetail(id, source);
    } else {
      this.errorMessage = 'Sipariş bilgisi eksik.';
      this.isLoading = false;
    }
  }

  loadOrderDetail(id: string, source: string): void {
    const serviceCall =
      source === 'Shopify'
        ? this.orderService.getShopifyOrderById(id)
        : this.orderService.getManualOrderById(id);

    serviceCall.subscribe({
      next: (data) => {
        this.order = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Sipariş detayı alınamadı.';
        this.isLoading = false;
      }
    });
  }
}
