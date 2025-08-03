import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { NotificationService, NotificationModel } from './services/notification.service';

/** Bildirimlerin listelendiği sayfa bileşeni */
@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html'
})
export class NotificationsComponent implements OnInit {
  notifications$!: Observable<NotificationModel[]>;

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.notifications$ = this.notificationService.notifications$;
    this.notificationService.loadNotifications();
  }
}
