import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ExcelToolsRoutingModule } from './excel-tools-routing.module';
import { ShopifyExportComponent } from './components/shopify-export/shopify-export.component';

@NgModule({
  declarations: [
    ShopifyExportComponent,   // ⬅️ declare ET
  ],
  imports: [
    CommonModule,
    FormsModule,              // ⬅️ ngModel için şart
    ReactiveFormsModule,      // (opsiyonel ama dursun)
    ExcelToolsRoutingModule,
  ],
})
export class ExcelToolsModule {}
