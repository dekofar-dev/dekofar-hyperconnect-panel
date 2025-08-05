import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';

export interface NotificationModel {
  id: string;
  title: string;
  message: string;
  type: string;
  createdAt: string;
  isRead?: boolean;
}

/**
 * Bildirim sistemi pasif sürüm (SignalR ve HTTP çağrıları kapalı)
 */
@Injectable({ providedIn: 'root' })
export class NotificationService {
  /** Boş bildirim listesi */
  private notificationsSubject = new BehaviorSubject<NotificationModel[]>([]);
  notifications$: Observable<NotificationModel[]> = this.notificationsSubject.asObservable();

  /** Okunmamış bildirim sayısı (hep 0) */
  private unreadCountSubject = new BehaviorSubject<number>(0);
  unreadCount$: Observable<number> = this.unreadCountSubject.asObservable();

  constructor() {}

  /**
   * SignalR bağlantısını başlatır (devre dışı)
   */
  startConnection(): void {
    // SignalR bağlantısı devre dışı
    // console.log('startConnection devre dışı');
  }

  /**
   * SignalR bağlantısını kapatır (devre dışı)
   */
  stopConnection(): void {
    // console.log('stopConnection devre dışı');
  }

  /**
   * Bildirimleri yükler (boş gönderir)
   */
  loadNotifications(): void {
    // API çağrısı yapılmadan boş liste gönderilir
    this.notificationsSubject.next([]);
    this.unreadCountSubject.next(0);
  }

  /**
   * Bildirimi okunmuş olarak işaretler (devre dışı)
   */
  markAsRead(id: string): void {
    // Devre dışı
    // console.log(`Bildirim ${id} okunmuş olarak işaretlendi (devre dışı).`);
  }
}
