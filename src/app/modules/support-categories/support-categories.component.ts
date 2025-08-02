import { Component, OnInit } from '@angular/core';
import { SupportCategoryService, SupportCategory } from './services/support-category.service';

@Component({
  selector: 'app-support-categories',
  templateUrl: './support-categories.component.html'
})
export class SupportCategoriesComponent implements OnInit {
  categories: SupportCategory[] = [];
  loading = false;

  constructor(private service: SupportCategoryService) {}

  ngOnInit(): void {
    this.fetch();
  }

  fetch(): void {
    this.loading = true;
    this.service.getAll().subscribe({
      next: (res) => {
        this.categories = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
