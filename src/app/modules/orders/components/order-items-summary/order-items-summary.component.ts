import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { ProductVariantSummary } from '../../models/product-variant-summary.model';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-order-items-summary',
  templateUrl: './order-items-summary.component.html'
})
export class OrderItemsSummaryComponent implements OnInit {

  // =====================================================
  // 🔹 DATA
  // =====================================================

  products: ProductVariantSummary[] = [];
  grandTotal = 0;

  // =====================================================
  // 🔍 SEARCH
  // =====================================================

  /** Kullanıcının yazdığı değer */
  tagSearch = new FormControl('');

  /** Backend’e gönderilen AKTİF tag */
  activeTag?: string;

  // =====================================================
  // 🔹 UI STATE
  // =====================================================

  loading = false;

  constructor(private orders: OrderService) {}

  // =====================================================
  // 🔹 LIFECYCLE
  // =====================================================

  ngOnInit(): void {
    // İlk yükleme (tagsiz)
    this.loadProducts();
  }

  // =====================================================
  // 🔹 LOAD PRODUCTS
  // =====================================================

  loadProducts(tag?: string): void {
    this.loading = true;

    this.orders.getProductVariantSummary(tag).subscribe({
      next: res => {
        this.products = res ?? [];

        this.grandTotal = this.products.reduce(
          (sum, p) => sum + (p.totalQuantity ?? 0),
          0
        );

        this.loading = false;
      },
      error: () => {
        this.products = [];
        this.grandTotal = 0;
        this.loading = false;
      }
    });
  }

  // =====================================================
  // 🔹 SEARCH ACTIONS
  // =====================================================

  /** Enter veya Ara butonu */
  search(): void {
    const raw = this.tagSearch.value ?? '';
    const tag = raw.trim();

    this.activeTag = tag || undefined;
    this.loadProducts(this.activeTag);
  }

  clearSearch(): void {
    this.tagSearch.setValue('');
    this.activeTag = undefined;
    this.loadProducts();
  }

  // =====================================================
  // 🔹 TRACK BY
  // =====================================================

  trackProduct = (_: number, p: ProductVariantSummary) =>
    p.productTitle;

  trackVariant = (_: number, v: any) =>
    `${v.variantTitle ?? 'standart'}-${v.quantity}`;
}
