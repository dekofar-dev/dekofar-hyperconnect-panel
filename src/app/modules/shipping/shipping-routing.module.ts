import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TrackCodeEntryComponent } from './track-code-entry/track-code-entry.component';

const routes: Routes = [
  {
    path: 'track-code-entry',
    component: TrackCodeEntryComponent
  }
  // ileride shipping/create-shipment, shipping/delivery-issues gibi path'leri buraya ekleyebilirsin
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ShippingRoutingModule {}
