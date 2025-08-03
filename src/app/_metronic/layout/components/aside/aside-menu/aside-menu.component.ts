import { Component, OnInit } from '@angular/core';
import { environment } from '../../../../../../environments/environment';
import { AuthService } from 'src/app/modules/auth';

// Menü öğesi tipleri
interface MenuItem {
  title: string; // Türkçe başlık
  icon: string; // Bootstrap ikon adı
  route: string; // Yönlendirme adresi
  roles?: string[]; // Rol bazlı görünürlük
  onlyGuests?: boolean; // Sadece oturum açmamış kullanıcılar için
}

interface MenuSection {
  header: string; // Grup başlığı
  items: MenuItem[]; // Gruptaki menüler
}

@Component({
  selector: 'app-aside-menu',
  templateUrl: './aside-menu.component.html',
  styleUrls: ['./aside-menu.component.scss'],
})
export class AsideMenuComponent implements OnInit {
  appAngularVersion: string = environment.appVersion;
  appPreviewChangelogUrl: string = environment.appPreviewChangelogUrl;
  userRole: string | null = null; // Oturumdaki kullanıcının rolü
  isLoggedIn = false; // Oturum durumunu tutar

  // Uygulamada tamamlanan modüller için menü yapısı
  menuSections: MenuSection[] = [
    {
      header: 'Yönetim', // Yönetim modülleri
      items: [
        { title: 'Kullanıcılar', icon: 'person', route: '/users', roles: ['Admin'] },
        { title: 'Roller & Yetkiler', icon: 'shield', route: '/roles', roles: ['Admin'] },
      ],
    },
    {
      header: 'Zaman Takibi', // Mesai modülü
      items: [{ title: 'Mesai Takibi', icon: 'clock', route: '/work-sessions' }],
    },
    {
      header: 'Hesabım', // Kullanıcıya özel işlemler
      items: [
        { title: 'Profilim', icon: 'person', route: '/profile' },
        { title: 'Bildirimler', icon: 'bell', route: '/notifications' },
        { title: 'Giriş Yap', icon: 'lock', route: '/auth/login', onlyGuests: true },
      ],
    },
  ];

  constructor(private auth: AuthService) {}

  ngOnInit(): void {
    const user = this.auth.getAuthFromLocalStorage();
    this.userRole = user?.role || null; // Rol bilgisini al
    this.isLoggedIn = !!user; // Kullanıcının oturum durumunu ayarla
  }
}
