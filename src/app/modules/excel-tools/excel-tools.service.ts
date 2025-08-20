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

  /** Telefonu 10 hane ve başında 0 olacak şekilde normalize eder -> 0XXXXXXXXX */
/** TR telefon: 10 hane, başında 0 yok (örn: 5364621515) */
private normalizePhone(input?: string | number | null): string {
  let s = String(input ?? '').replace(/\D/g, '');
  if (!s) return '';

  // +90 / 90 kırp
  if (s.startsWith('90') && s.length >= 12) s = s.slice(2);

  // 0 ile başlıyorsa ve 11+ hane ise 0'ı kırp (0536... -> 536...)
  if (s.startsWith('0') && s.length >= 11) s = s.slice(1);

  // Son 10 haneyi al
  if (s.length >= 10) return s.slice(-10);

  // 10 haneden kısa ise olduğu gibi dön (çok nadir durum)
  return s;
}

  /**
   * Siparişleri DHL toplu yükleme şablonuna dönüştürür.
   * Zorunlu: REFERANS_ID, ALICI_ADI, ADRES, IL, ILCE, TELEFON, KARGO_ICERIK, ADET, KIYMET
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
// Excel'de IL ve ILCE tersine yazılsın:
const il   = (o.district ?? '').trim(); // IL kolonuna district
const ilce = (o.city ?? '').trim();     // ILCE kolonuna city


      const phone = this.normalizePhone(o.phone);
      const email = (o.email ?? '').trim();

      const firstItemName =
        (o.items?.[0]?.productName ?? o.items?.[0]?.variantName ?? '').trim() || 'Ürün';

const aciklama =
  (o.items?.[0]?.productName ?? o.items?.[0]?.variantName ?? '').trim() || 'Ürün';
      const irsaliye = ref; // sipariş no
      const kiymet = Number(o.totalAmount ?? 0);

      return [
        ref,                          // REFERANS_ID
        '',                           // ALICI_BAYI_NO
        name,                         // ALICI_ADI
        addr,                         // ADRES
        il,                           // IL
        ilce,                         // ILCE
        phone,                        // TELEFON (10 hane, başında 0)
        phone,                        // TELEFON_CEP
        email,                        // EMAIL
        ExcelToolsService.VERGI_TC_NO,// KIMLIK_VERGI_NO (sabit)
        '',                           // VERGI_DAIRESI
        firstItemName,                // KARGO_ICERIK (ürün adı)
        1,                            // ADET (sabit 1)
        kilo,                         // KILO (sabit 3)
        desi,                         // DESI (sabit 5)
        odemeTipi,                    // ODEME_TIPI_A_G ('G')
        teslimSekli,                  // TESLIM_SEKLI_AH_AT_TI ('AT')
        aciklama,                     // ACIKLAMA
        irsaliye,                     // IRSALIYE_NO (sipariş no)
        kiymet,                       // KIYMET (totalAmount)
        kapidaTahsilat,               // KAPIDA_TAHSILAT ('E')
        aliciSms,                     // ALICI_SMS_E_H
        gondericiSms,                 // GONDERICI_SMS_E_H
        '',                           // PARCA_DAGILIMI
        '',                           // PLATFORM_KISA_KODU
        '',                           // PLATFORM_SATIS_KODU
        ''                            // SIPARIS_BARKOD
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
}
