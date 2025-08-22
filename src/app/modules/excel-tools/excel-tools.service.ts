import { Injectable } from '@angular/core';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';
import { OrderModel } from '../orders/models/order.model';

export type EH = 'E' | 'H';
export type AG = 'A' | 'G';
export type TeslimSekli = 'AH' | 'AT' | 'TI';

export interface DhlExportOptions {
  kilo?: number;             // default 3
  desi?: number;             // default 5
  odemeTipi?: AG;            // default 'G'
  teslimSekli?: TeslimSekli; // default 'AT'
  kapidaTahsilat?: EH;       // default 'E'
  aliciSms?: EH;             // default 'E'
  gondericiSms?: EH;         // default 'E'
}

@Injectable({ providedIn: 'root' })
export class ExcelToolsService {
  private static readonly VERGI_TC_NO = '29440840854';

/** Ürün kısa adları */
private static readonly PRODUCT_SHORT_NAMES: Record<string, string> = {
  "7908059578422": "Sarı Siyah tekli",
  "7908952703030": "Rose Cam",
  "7907399827510": "Uyku Tulumu 200x70",
  "7905164492854": "Gint",
  "7907989618742": "Çaydanlık 3+1.5L",
  "7908058824758": "Çaydanlık 3+2L",
  "7908023697462": "Çaydanlık 3+2L Şık",
  "7907974774838": "Çaydanlık 3+2L Sarı",
  "7907402285110": "4L Termos",
  "7908060397622": "safe",
  "7907396550710": "Gri Cam",
  "7907398221878": "Sandalet Deri",
  "7907942072374": "Çaydanlık 2.5+1L",
  "7907395436598": "Çelik Çaycı",
  "7907575005238": "Taktik Çanta",
};


  /** Telefonu normalize eder -> 10 hane, başında 0 olacak şekilde */
  private normalizePhone(input?: string | number | null): string {
    let s = String(input ?? '').replace(/\D/g, '');
    if (!s) return '';

    if (s.startsWith('90') && s.length >= 12) s = s.slice(2);
    if (s.startsWith('0') && s.length >= 11) s = s.slice(1);
    if (s.length >= 10) return s.slice(-10);

    return s;
  }

  /** Ürün adı kısaltma + varyant + adet */
  private getShortName(productId?: string, variantName?: string, qty?: number): string {
    const base = productId
      ? ExcelToolsService.PRODUCT_SHORT_NAMES[productId] || 'Ürün'
      : 'Ürün';
    const variant = variantName ? ` ${variantName}` : '';
    const count = qty && qty > 1 ? ` x${qty}` : '';
    return base + variant + count;
  }

  /**
   * Siparişleri DHL toplu yükleme şablonuna dönüştürür.
   */
  exportDhlBatch(orders: OrderModel[], fileName: string, opt: DhlExportOptions = {}) {
    const {
      kilo = 3,
      desi = 5,
      odemeTipi = 'G',
      teslimSekli = 'AT',
      kapidaTahsilat = 'E',
      aliciSms = 'E',
      gondericiSms = 'E',
    } = opt;

    const headers = [
      'REFERANS_ID','ALICI_BAYI_NO','ALICI_ADI','ADRES','IL','ILCE','TELEFON','TELEFON_CEP','EMAIL',
      'KIMLIK_VERGI_NO','VERGI_DAIRESI','KARGO_ICERIK','ADET','KILO','DESI','ODEME_TIPI_A_G',
      'TESLIM_SEKLI_AH_AT_TI','ACIKLAMA','IRSALIYE_NO','KIYMET','KAPIDA_TAHSILAT','ALICI_SMS_E_H',
      'GONDERICI_SMS_E_H','PARCA_DAGILIMI','PLATFORM_KISA_KODU','PLATFORM_SATIS_KODU','SIPARIS_BARKOD'
    ];

    const rows = (orders || []).map((o) => {
      const ref = o.orderNumber ?? (o as any).order_number ?? o.id ?? '';
      const name = (o.customerName ?? '').trim() || 'Müşteri';
      const addr = (o.address ?? '').trim();

      // Excel’de IL ve ILCE ters yazılacak
      const il   = (o.district ?? '').trim(); // IL kolonuna district
      const ilce = (o.city ?? '').trim();     // ILCE kolonuna city

      const phone = this.normalizePhone(o.phone);
      const email = (o.email ?? '').trim();

      const firstItem = o.items?.[0];
      const kargoIcerik = this.getShortName(firstItem?.productId, firstItem?.variantName, firstItem?.quantity);
      const aciklama = kargoIcerik;
      const irsaliye = ref;
      const kiymet = Number(o.totalAmount ?? 0);

      return [
        ref,                          // REFERANS_ID
        '',                           // ALICI_BAYI_NO
        name,                         // ALICI_ADI
        addr,                         // ADRES
        il,                           // IL
        ilce,                         // ILCE
        phone,                        // TELEFON
        phone,                        // TELEFON_CEP
        email,                        // EMAIL
        ExcelToolsService.VERGI_TC_NO,// KIMLIK_VERGI_NO
        '',                           // VERGI_DAIRESI
        kargoIcerik,                  // KARGO_ICERIK
        firstItem?.quantity ?? 1,     // ADET
        kilo,                         // KILO
        desi,                         // DESI
        odemeTipi,                    // ODEME_TIPI_A_G
        teslimSekli,                  // TESLIM_SEKLI_AH_AT_TI
        aciklama,                     // ACIKLAMA
        irsaliye,                     // IRSALIYE_NO
        kiymet,                       // KIYMET
        kapidaTahsilat,               // KAPIDA_TAHSILAT
        aliciSms,                     // ALICI_SMS_E_H
        gondericiSms,                 // GONDERICI_SMS_E_H
        '', '', '', ''                // diğer kolonlar boş
      ];
    });

    const ws = XLSX.utils.aoa_to_sheet([headers, ...rows]);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'DHL');

    const wbout = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
    saveAs(new Blob([wbout], { type: 'application/octet-stream' }), fileName);
  }

  // kısa yol
  exportShopifyToTemplate(orders: OrderModel[], fileName: string) {
    this.exportDhlBatch(orders, fileName);
  }



exportSimple(rows: any[], fileName: string): void {
  const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(rows);
  const wb: XLSX.WorkBook = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(wb, ws, 'Rehber');
  const buf = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
  const blob = new Blob([buf], { type: 'application/octet-stream' });
  saveAs(blob, fileName);
}



}
