import { Component, inject, signal, OnInit } from '@angular/core';
import { OrderService } from 'src/app/modules/orders/services/order.service';
import { ExcelToolsService } from '../../excel-tools.service';

@Component({
  selector: 'app-order-contact-export',
  templateUrl: './order-contact-export.component.html',
  styleUrls: ['./order-contact-export.component.scss'],
})
export class OrderContactExportComponent implements OnInit {
  private orderService = inject(OrderService);
  private excel = inject(ExcelToolsService);

  loading24 = signal(false);
  loading7 = signal(false);

  contacts24 = signal<{ name: string; phone: string }[]>([]);
  contacts7 = signal<{ name: string; phone: string }[]>([]);

  ngOnInit(): void {
    this.load(1); // 24 saat
    this.load(7); // 7 gün
  }

  private load(days: number): void {
    const is24 = days === 1;
    if (is24) this.loading24.set(true); else this.loading7.set(true);

    const since = new Date();
    since.setDate(since.getDate() - days);
    const iso = since.toISOString();
    const q = `created_at:>=${iso} AND status:open`;

    this.orderService.searchOrderContactsShopifyOnly(q).subscribe({
      next: (list) => {
        if (is24) this.contacts24.set(list);
        else this.contacts7.set(list);

        if (is24) this.loading24.set(false);
        else this.loading7.set(false);
      },
      error: (err) => {
        console.error(err);
        if (is24) this.loading24.set(false);
        else this.loading7.set(false);
      }
    });
  }

  exportExcel(days: number): void {
    const data = days === 1 ? this.contacts24() : this.contacts7();
    if (!data.length) return;

    const rows = data.map(x => ({
      'Ad Soyad': x.name,
      'Telefon': x.phone
    }));

    const fileName = days === 1
      ? `Son24Saat_Kontaklar_${new Date().toISOString().slice(0,10)}.xlsx`
      : `Son7Gun_Kontaklar_${new Date().toISOString().slice(0,10)}.xlsx`;

    this.excel.exportSimple(rows, fileName);
  }
}
