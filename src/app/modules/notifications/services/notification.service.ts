import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../../auth/services/auth.service';

export interface NotificationModel {
  id: string;
  title: string;
  message: string;
  type: string;
  createdAt: string;
  isRead?: boolean;
}

/**
 * SignalR üzerinden gerçek zamanlı bildirimleri yöneten servis
 */
@Injectable({ providedIn: 'root' })
export class NotificationService {
  private hubConnection?: HubConnection;
  private notificationsSubject = new BehaviorSubject<NotificationModel[]>([]);
  /** Bildirim listesine abonelik */
  notifications$: Observable<NotificationModel[]> = this.notificationsSubject.asObservable();
  private unreadCountSubject = new BehaviorSubject<number>(0);
  /** Okunmamış bildirim sayısı */
  unreadCount$: Observable<number> = this.unreadCountSubject.asObservable();

  constructor(private http: HttpClient, private auth: AuthService) {}

  /**
   * SignalR bağlantısını başlatır
   */
  startConnection(): void {
    if (this.hubConnection) return;
    const baseUrl = environment.apiUrl.replace('/api', '');
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${baseUrl}/hubs/notification`, {
        accessTokenFactory: () => this.auth.getToken() || ''
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    // Sunucudan gelen yeni bildirimleri dinle
    this.hubConnection.on('ReceiveNotification', (notif: NotificationModel) => {
      const current = this.notificationsSubject.getValue();
      this.notificationsSubject.next([notif, ...current]);
      const unread = this.unreadCountSubject.getValue() + 1;
      this.unreadCountSubject.next(unread);
    });

    this.hubConnection
      .start()
      .catch(err => console.error('SignalR bağlantısı kurulamadı:', err));
  }

  /** Bağlantıyı sonlandırır */
  stopConnection(): void {
    this.hubConnection?.stop();
    this.hubConnection = undefined;
  }

  /**
   * Sunucudan son bildirimleri çeker
   */
  loadNotifications(): void {
    this.http
      .get<NotificationModel[]>(`${environment.apiUrl}/notifications`)
      .subscribe(list => {
        this.notificationsSubject.next(list);
        const unread = list.filter(n => !n.isRead).length;
        this.unreadCountSubject.next(unread);
      });
  }

  /**
   * Bildirimi okunmuş olarak işaretler
   */
  markAsRead(id: string): void {
    this.http
      .post(`${environment.apiUrl}/notifications/${id}/read`, {})
      .subscribe(() => {
        const list = this.notificationsSubject.getValue().map(n =>
          n.id === id ? { ...n, isRead: true } : n
        );
        this.notificationsSubject.next(list);
        const unread = list.filter(n => !n.isRead).length;
        this.unreadCountSubject.next(unread);
      });
  }
}

