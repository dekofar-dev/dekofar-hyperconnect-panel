import { Component, OnInit, HostBinding } from '@angular/core';
import { NotificationService, NotificationModel } from '../../../../../../modules/notifications/services/notification.service';

/** Topbar bildirim açılır içeriği */
@Component({
  selector: 'app-notifications-inner',
  templateUrl: './notifications-inner.component.html'
})
export class NotificationsInnerComponent implements OnInit {
  @HostBinding('class')
  class = 'menu menu-sub menu-sub-dropdown menu-column w-350px w-lg-375px';
  @HostBinding('attr.data-kt-menu')
  dataKtMenu = 'true';

  notifications: NotificationModel[] = [];

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.notificationService.notifications$.subscribe(list => (this.notifications = list));
  }

  /** Bildirimi okundu olarak işaretle */
  markAsRead(id: string): void {
    this.notificationService.markAsRead(id);
  }
}
