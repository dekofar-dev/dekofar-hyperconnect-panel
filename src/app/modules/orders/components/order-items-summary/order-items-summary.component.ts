import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { OrderItemSummary, ProductGroup, VariantRow } from '../../models/order-item-summary.model';

@Component({
  selector: 'app-order-items-summary',
  templateUrl: './order-items-summary.component.html'
})
export class OrderItemsSummaryComponent implements OnInit {
  // ham api verisi
  items: OrderItemSummary[] = [];

  // ekranda kullanılacak gruplu veri
  groups: ProductGroup[] = [];

  // sayfanın en altında gösterilecek genel toplam
  grandTotal = 0;

  // UI durumu
  loading = false;

  constructor(private orders: OrderService) {}

  ngOnInit(): void {
    this.load(30); // varsayılan 30 gün
  }

  /** API'den özeti çek ve ürün bazında grupla */
  load(days = 30): void {
    this.loading = true;

    // OrderService.getShopifyOrderItemsSummary default filtreleri:
    // status=open, financial=pending,authorized,paid,partially_paid,partially_refunded, fulfillment=unfulfilled,partial
    this.orders.getShopifyOrderItemsSummary({ days })
      .subscribe({
        next: (res) => {
          this.items = res ?? [];
          this.buildGroups();
          this.loading = false;
        },
        error: () => {
          this.items = [];
          this.groups = [];
          this.grandTotal = 0;
          this.loading = false;
        }
      });
  }

  /** Gelen düz listeyi ürün bazında grupla ve varyantları altına topla */
  private buildGroups(): void {
    const map = new Map<number, ProductGroup>();
    let total = 0;

    for (const it of this.items) {
      total += it.totalQuantity;

      const existing = map.get(it.productId);
      if (!existing) {
        map.set(it.productId, {
          productId: it.productId,
          title: it.title,
          imageUrl: it.imageUrl,
          totalQuantity: it.totalQuantity,
          variants: [{
            variantId: it.variantId,
            variantTitle: it.variantTitle,
            quantity: it.totalQuantity,
            imageUrl: it.imageUrl
          }]
        });
      } else {
        // Ürün toplamı
        existing.totalQuantity += it.totalQuantity;

        // Varyantı bul ya da ekle (id + title kombinasyonu ile)
        const idx = existing.variants.findIndex(v =>
          (v.variantId ?? 0) === (it.variantId ?? 0) &&
          (v.variantTitle ?? '') === (it.variantTitle ?? '')
        );
        if (idx >= 0) {
          existing.variants[idx].quantity += it.totalQuantity;
        } else {
          existing.variants.push({
            variantId: it.variantId,
            variantTitle: it.variantTitle,
            quantity: it.totalQuantity,
            imageUrl: it.imageUrl
          });
        }

        // Ürün görseli boşsa ilk bulunanı ata
        if (!existing.imageUrl && it.imageUrl) {
          existing.imageUrl = it.imageUrl;
        }
      }
    }

    // Sıralamalar: varyantlar ve ürünler adetine göre azalan
    const list = Array.from(map.values());
    for (const g of list) {
      g.variants.sort((a, b) => b.quantity - a.quantity);
    }
    list.sort((a, b) => b.totalQuantity - a.totalQuantity);

    this.groups = list;
    this.grandTotal = total;
  }

  /** *ngFor trackBy */
  trackGroup = (_: number, g: ProductGroup) => g.productId;
  trackVariant = (_: number, v: VariantRow) => `${v.variantId ?? 0}-${v.variantTitle ?? ''}`;
}
