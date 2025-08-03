import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AuthService } from '../auth/services/auth.service';

/** Kullanıcı profilinin ana bileşeni */
@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
})
export class ProfileComponent implements OnInit {
  isOnline = false;
  lastSeen?: string;
  unreadMessageCount = 0;

  constructor(private http: HttpClient, private auth: AuthService) {}

  ngOnInit(): void {
    const user = this.auth.getAuthFromLocalStorage();
    if (user) {
      this.http.get<{ isOnline: boolean; lastSeen: string | null }>(`${environment.apiUrl}/status/${user.id}`).subscribe(res => {
        this.isOnline = res.isOnline;
        this.lastSeen = res.lastSeen || undefined;
      });
      this.http.get<number>(`${environment.apiUrl}/usermessages/unread-count`).subscribe(c => (this.unreadMessageCount = c));
    }
  }
}
