import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ExcelToolsRoutingModule } from './excel-tools-routing.module';
import { ShopifyExportComponent } from './components/shopify-export/shopify-export.component';
import { OrderContactExportComponent } from './components/order-contact-export/order-contact-export.component';

@NgModule({
  declarations: [
    ShopifyExportComponent,   
    OrderContactExportComponent
  ],
  imports: [
    CommonModule,
    FormsModule,              
    ReactiveFormsModule,  
    ExcelToolsRoutingModule,
  ],
})
export class ExcelToolsModule {}
