import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';   // ✅ ekle
import { ShippingRoutingModule } from './shipping-routing.module';
import { TrackCodeEntryComponent } from './track-code-entry/track-code-entry.component';

@NgModule({
  declarations: [TrackCodeEntryComponent  ],
  imports: [
    CommonModule,
    FormsModule,           // ✅ ekle
    ShippingRoutingModule
  ]
})
export class ShippingModule {}
