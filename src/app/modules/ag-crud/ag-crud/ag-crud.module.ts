// ag-crud.module.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgGridModule } from 'ag-grid-angular';
import { AgCrudComponent } from './ag-crud.component';

@NgModule({
  declarations: [AgCrudComponent],
  imports: [
    CommonModule,
    AgGridModule // AG Grid bileşeni
  ],
  exports: [AgCrudComponent]
})
export class AgCrudModule {}
