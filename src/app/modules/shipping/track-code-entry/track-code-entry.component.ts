import { Component } from '@angular/core';
import {
  ShippingService,
  ShipmentStatus,
  ShipmentTrack,
  ShipmentDetail,
} from '../shipping.service';

@Component({
  selector: 'app-track-code-entry',
  templateUrl: './track-code-entry.component.html',
})
export class TrackCodeEntryComponent {
  shipmentId: string = '';

  status?: ShipmentStatus;
  tracks: ShipmentTrack[] = [];
  detail?: ShipmentDetail;

  loading = false;
  error?: string;

  constructor(private shippingService: ShippingService) {}

  // ---- Helpers: tarih parse & sıralama ----
  private toTimestamp(date: Date | null): number {
    return date ? date.getTime() : 0;
  }

  /** dd.MM.yyyy HH:mm veya dd.MM.yyyy formatlarını parse eder; ISO'yu da dener */
  private parseTurkishDate(input: string | undefined | null): Date | null {
    if (!input) return null;

    // 22.08.2025 14:05 veya 22.08.2025
    const m = input.trim().match(
      /^(\d{2})\.(\d{2})\.(\d{4})(?:\s+(\d{2}):(\d{2}))?$/
    );
    if (m) {
      const dd = parseInt(m[1], 10);
      const mm = parseInt(m[2], 10) - 1; // 0-based
      const yyyy = parseInt(m[3], 10);
      const HH = m[4] ? parseInt(m[4], 10) : 0;
      const MM = m[5] ? parseInt(m[5], 10) : 0;
      return new Date(yyyy, mm, dd, HH, MM, 0);
    }

    // ISO benzeri tarihleri de dene
    const d = new Date(input);
    return isNaN(d.getTime()) ? null : d;
  }

  /** Track içindeki olası alanlardan en iyi zamanı üretir */
  private trackToTimestamp(t: ShipmentTrack): number {
    // 1) eventDateTime (ISO olabiliyor)
    const dt1 = (t as any).eventDateTime as string | undefined;
    if (dt1) {
      const d = new Date(dt1);
      if (!isNaN(d.getTime())) return d.getTime();
    }

    // 2) eventDateTime2 (genelde dd.MM.yyyy HH:mm)
    const dt2 = (t as any).eventDateTime2 as string | undefined;
    if (dt2) {
      const d = this.parseTurkishDate(dt2);
      if (d) return d.getTime();
    }

    // 3) Ayrı alanlar: eventDate + eventTime
    const dOnly = (t as any).eventDate as string | undefined;
    const tOnly = (t as any).eventTime as string | undefined;
    if (dOnly) {
      const combined = tOnly ? `${dOnly} ${tOnly}` : dOnly;
      const d = this.parseTurkishDate(combined);
      if (d) return d.getTime();
    }

    // 4) Son çare: description içinde tarih varsa
    const desc = (t as any).description as string | undefined;
    if (desc) {
      const m = desc.match(/(\d{2}\.\d{2}\.\d{4}(?:\s+\d{2}:\d{2})?)/);
      if (m?.[1]) {
        const d = this.parseTurkishDate(m[1]);
        if (d) return d.getTime();
      }
    }

    return 0; // bilinmeyen -> en eski gibi davran
  }

  search() {
    if (!this.shipmentId || this.shipmentId.trim() === '') {
      this.error = 'Lütfen takip numarası giriniz';
      return;
    }

    // Reset
    this.loading = true;
    this.error = undefined;
    this.status = undefined;
    this.tracks = [];
    this.detail = undefined;

    // 1) Son durum
    this.shippingService.getShipmentStatus(this.shipmentId).subscribe({
      next: (res) => {
        this.status = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = '⚠️ Gönderi bulunamadı veya hatalı takip kodu';
      },
    });
// 2) Hareket geçmişi (SON DURUM İLK olacak şekilde kesin sıralama)
this.shippingService.trackShipment(this.shipmentId).subscribe({
  next: (res) => {
    const withTs = (res || [])
      .filter(
        (t: any) =>
          t?.eventDateTime ||
          t?.eventDateTime2 ||
          t?.eventDate ||
          t?.description
      )
      .map((t: any) => ({ ...t, _ts: this.trackToTimestamp(t) }))
      .sort((a: any, b: any) => b._ts - a._ts);

    // _ts'i dışarı sızdırmadan orijinal tipe döndür
    this.tracks = withTs.map(({ _ts, ...rest }: any) => rest) as ShipmentTrack[];

    // BUGFIX: this.status'a referans vermeden set et
    if (!this.status && withTs.length > 0) {
      const top = withTs[0] as any;
      this.status = {
        // ShipmentStatus içinde başka zorunlu alan yoksa bu yeterli
        shipmentStatusExplanation: (top.eventStatus as string) ?? ''
      } as ShipmentStatus;
    }
  },
  error: () => {},
});

    // 3) Gönderi detay
    this.shippingService.getShipmentDetail(this.shipmentId).subscribe({
      next: (res) => {
        this.detail = res;
      },
      error: () => {},
    });
  }
}
