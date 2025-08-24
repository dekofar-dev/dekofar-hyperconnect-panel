import { Component } from '@angular/core';
import { 
  ShippingService, 
  ShipmentStatus, 
  ShipmentTrack, 
  ShipmentDetail 
} from '../shipping.service';

@Component({
  selector: 'app-track-code-entry',
  templateUrl: './track-code-entry.component.html'
})
export class TrackCodeEntryComponent {
  shipmentId: string = '';

  status?: ShipmentStatus;        // Son durum bilgisi
  tracks: ShipmentTrack[] = [];   // Hareket geçmişi
  detail?: ShipmentDetail;        // Detaylı gönderi bilgisi

  loading = false;
  error?: string;

  constructor(private shippingService: ShippingService) {}

  search() {
    if (!this.shipmentId || this.shipmentId.trim() === '') {
      this.error = 'Lütfen takip numarası giriniz';
      return;
    }

    // Reset state
    this.loading = true;
    this.error = undefined;
    this.status = undefined;
    this.tracks = [];
    this.detail = undefined;

    // 1) DHL/MNG son durumu getir
    this.shippingService.getShipmentStatus(this.shipmentId).subscribe({
      next: (res) => {
        this.status = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = '⚠️ Gönderi bulunamadı veya hatalı takip kodu';
      }
    });

    // 2) Hareket geçmişi
    this.shippingService.trackShipment(this.shipmentId).subscribe({
      next: (res) => {
        this.tracks = res;
      },
      error: () => {}
    });

    // 3) Gönderi detay bilgisi
    this.shippingService.getShipmentDetail(this.shipmentId).subscribe({
      next: (res) => {
        this.detail = res;
      },
      error: () => {}
    });
  }
}
