import { Component, inject, signal, OnInit } from '@angular/core';
import { OrderService } from 'src/app/modules/orders/services/order.service';
import { OrderModel } from 'src/app/modules/orders/models/order.model';
import { ExcelToolsService } from '../../excel-tools.service';

type CarrierFilter = 'DHL' | 'PTT' | 'ARAS' | 'ONAY';

@Component({
  selector: 'app-shopify-export',
  templateUrl: './shopify-export.component.html',
  styleUrls: ['./shopify-export.component.scss'],
})
export class ShopifyExportComponent implements OnInit {
  private orderService = inject(OrderService);
  private excel = inject(ExcelToolsService);

  // UI state
  loading = signal(false);
  error = signal<string | null>(null);
  orders = signal<OrderModel[]>([]);
  filter = signal<CarrierFilter>('DHL'); // varsayılan

  ngOnInit(): void {
    this.load();
  }

  private buildQuery(): string {
    const parts = ['status:open', 'fulfillment_status:unfulfilled'];
    const f = this.filter();
    if (f === 'DHL') parts.push('tag:DHL');
    if (f === 'PTT') parts.push('tag:PTT');
    if (f === 'ARAS') parts.push('tag:ARAS');
    if (f === 'ONAY') parts.push('tag:ONAY');
    return parts.join(' AND ');
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);

    const q = this.buildQuery();

    // Gelişmiş: önce arama ile ID’ler, sonra detay endpoint’leri (backend: /api/Shopify/orders/{id})
    this.orderService.searchOrdersDetailedFrontOnly(q, 50).subscribe({
      next: (items) => {
        this.orders.set(items.filter((x) => x.source === 'Shopify'));
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.error.set('Siparişler alınırken bir hata oluştu.');
        this.loading.set(false);
      },
    });
  }

  exportExcel(): void {
    const suffix = this.filter().toLowerCase();
    this.excel.exportDhlBatch(
      this.orders(),
      `${suffix}-unfulfilled_${new Date().toISOString().slice(0, 10)}.xlsx`,
      {
        kilo: 3,
        desi: 5,
        kapidaTahsilat: 'E', // kapıda tahsilat var
        odemeTipi: 'G',      // gönderici öder
        teslimSekli: 'AT',   // teslim şekli AT
      }
    );
  }

  onFilterChange(value: CarrierFilter): void {
    this.filter.set(value);
    this.load();
  }

  trackById = (_: number, item: OrderModel) => item.id;
}
