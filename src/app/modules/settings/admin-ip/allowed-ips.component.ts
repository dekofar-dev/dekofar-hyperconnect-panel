import { Component, OnInit } from '@angular/core';
import { AllowedAdminIpsService, AllowedAdminIp } from './services/allowed-admin-ips.service';

// Yetkili IP adreslerini yöneten bileşen
@Component({
  selector: 'app-allowed-ips',
  templateUrl: './allowed-ips.component.html'
})
export class AllowedIpsComponent implements OnInit {
  ips: AllowedAdminIp[] = [];
  newIp = '';

  constructor(private service: AllowedAdminIpsService) {}

  ngOnInit(): void {
    this.load();
  }

  // IP listesini yükler
  load(): void {
    this.service.getAll().subscribe((res) => (this.ips = res));
  }

  // Yeni IP ekler
  add(): void {
    if (!this.newIp) {
      return;
    }
    this.service.addIp(this.newIp).subscribe(() => {
      this.newIp = '';
      this.load();
    });
  }

  // IP'yi listeden kaldırır
  remove(id: number): void {
    this.service.removeIp(id).subscribe(() => this.load());
  }
}
