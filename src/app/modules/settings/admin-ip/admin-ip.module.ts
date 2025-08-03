import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminIpRoutingModule } from './admin-ip-routing.module';
import { AllowedIpsComponent } from './allowed-ips.component';

// Yetkili IP'ler modülü
@NgModule({
  declarations: [AllowedIpsComponent],
  imports: [CommonModule, FormsModule, AdminIpRoutingModule],
})
export class AdminIpModule {}
