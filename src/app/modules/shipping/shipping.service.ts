import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

// --- DTO Modelleri ---

export interface ShipmentStatus {
  orderId?: string;
  referenceId?: string;
  shipmentId?: string;
  shipmentSerialandNumber?: string;
  shipmentDateTime?: string;
  shipmentStatus?: string;
  shipmentStatusCode?: number;
  shipmentStatusExplanation?: string;
  statusDateTime?: string;
  trackingUrl?: string;
  isDelivered?: number;
  deliveryDateTime?: string;
  deliveryTo?: string;
  retrieveShipmentId?: string;
}

export interface ShipmentTrack {
  referenceId?: string;
  eventSequence?: string;
  eventStatus?: string;
  eventStatusEn?: string;
  eventDateTime?: string;
  eventDateTimeFormat?: string;
  eventDateTimezone?: string;
  eventDateTime2?: string;
  eventDateTime2Format?: string;
  eventDateTime2zone?: string;
  location?: string;
  country?: string;
  locationAddress?: string;
  locationPhone?: string;
  deliveryDateTime?: string;
  deliveryTo?: string;
  description?: string;
  pieceTotal?: string;
}

export interface ShipmentInfo {
  shipmentServiceType?: string;
  packagingType?: string;
  paymentType?: string;
  deliveryType?: string;
  referenceId?: string;
  shipmentId?: string;
  shipmentSerialNumber?: string;
  shipmentNumber?: string;
  shipmentDateTime?: string;
  pieceCount?: number | null;
  totalKg?: number | null;
  totalDesi?: number | null;
  totalKgDesi?: number | null;
  total?: number | null;
  kdv?: number | null;
  finalTotal?: number | null;
  shipmentStatusCode?: number | null;
  isMarketPlaceShipment?: number | null;
  isMarketPlacePays?: number | null;
  shipperBranch?: string;
  billOfLandingId?: string;
  isCOD?: number | null;
  codAmount?: string;
  content?: string;
  estimatedDeliveryDate?: string;
  isDelivered?: number | null;
}

export interface ShipmentPiece {
  numberOfPieces?: number | null;
  kgDesi?: number | null;
  barcode?: string;
  desi?: number | null;
  kg?: number | null;
  content?: string;
}

export interface Shipper {
  customerId?: number | null;
  refCustomerId?: string;
  city?: string;
  district?: string;
  address?: string;
  bussinessPhoneNumber?: string;
  email?: string;
  taxOffice?: string;
  taxNumber?: string;
  fullName?: string;
  homePhoneNumber?: string;
  mobilePhoneNumber?: string;
}

export interface Recipient {
  refCustomerId?: string;
  city?: string;
  district?: string;
  address?: string;
  bussinessPhoneNumber?: string;
  email?: string;
  taxOffice?: string;
  taxNumber?: string;
  fullName?: string;
  homePhoneNumber?: string;
  mobilePhoneNumber?: string;
}

export interface ShipmentDetail {
  shipment?: ShipmentInfo;
  shipmentPieceList?: ShipmentPiece[];
  shipper?: Shipper;
  recipient?: Recipient;
}

@Injectable({
  providedIn: 'root'
})
export class ShippingService {
  private apiUrl = `${environment.apiUrl}/kargo/dhl/standardquery`;

  constructor(private http: HttpClient) {}

  /** Tek bir shipmentId için son durumu getirir */
  getShipmentStatus(shipmentId: string): Observable<ShipmentStatus> {
    return this.http.get<ShipmentStatus>(
      `${this.apiUrl}/shipment-status/by-shipmentid/${shipmentId}`
    );
  }

  /** Tek bir shipmentId için hareket geçmişini (track) getirir */
  trackShipment(shipmentId: string): Observable<ShipmentTrack[]> {
    return this.http.get<ShipmentTrack[]>(
      `${this.apiUrl}/track/by-shipmentid/${shipmentId}`
    );
  }

  /** Tek bir shipmentId için detaylı gönderi bilgisini getirir */
  getShipmentDetail(shipmentId: string): Observable<ShipmentDetail[]> {
    return this.http.get<ShipmentDetail[]>(
      `${this.apiUrl}/shipment/by-shipmentid/${shipmentId}`
    );
  }
}
