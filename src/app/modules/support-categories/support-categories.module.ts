import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SupportCategoriesRoutingModule } from './support-categories-routing.module';
import { SupportCategoryListComponent } from './components/support-category-list/support-category-list.component';
import { SupportCategoryFormComponent } from './components/support-category-form/support-category-form.component';

import { AgGridModule } from 'ag-grid-angular';
import { AgCrudComponent } from '../ag-crud/ag-crud/ag-crud.component';
import { AgCrudModule } from '../ag-crud/ag-crud/ag-crud.module';

@NgModule({
  declarations: [
    SupportCategoryListComponent,
    SupportCategoryFormComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SupportCategoriesRoutingModule,
    AgCrudModule,
  ]
})
export class SupportCategoriesModule {}
