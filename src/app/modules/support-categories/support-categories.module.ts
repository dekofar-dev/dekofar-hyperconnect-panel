import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SupportCategoriesRoutingModule } from './support-categories-routing.module';
import { SupportCategoriesComponent } from './support-categories.component';

@NgModule({
  declarations: [SupportCategoriesComponent],
  imports: [CommonModule, FormsModule, SupportCategoriesRoutingModule]
})
export class SupportCategoriesModule {}
