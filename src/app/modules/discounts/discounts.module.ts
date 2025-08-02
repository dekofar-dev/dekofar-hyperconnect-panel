import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DiscountsRoutingModule } from './discounts-routing.module';
import { DiscountsComponent } from './discounts.component';

@NgModule({
  declarations: [DiscountsComponent],
  imports: [CommonModule, FormsModule, DiscountsRoutingModule]
})
export class DiscountsModule {}
