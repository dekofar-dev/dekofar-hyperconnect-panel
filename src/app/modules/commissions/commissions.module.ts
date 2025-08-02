import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommissionsRoutingModule } from './commissions-routing.module';
import { CommissionsComponent } from './commissions.component';

@NgModule({
  declarations: [CommissionsComponent],
  imports: [CommonModule, CommissionsRoutingModule]
})
export class CommissionsModule {}
