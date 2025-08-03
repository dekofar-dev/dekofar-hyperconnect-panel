import { Component, OnInit } from '@angular/core';
import { SupportCategoryService } from './services/support-category.service';
import { SupportCategory } from './models/support-category.model';

@Component({
  selector: 'app-support-categories',
  templateUrl: './support-categories.component.html'
})
export class SupportCategoriesComponent implements OnInit {
  categories: SupportCategory[] = [];
  loading = false;
  newName = '';

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
      error: () => (this.loading = false)
    });
  }

  create(): void {
    if (!this.newName.trim()) return;
    this.service.create({ name: this.newName }).subscribe({
      next: () => {
        this.newName = '';
        this.fetch();
      }
    });
  }

  update(category: SupportCategory): void {
    this.service.update(category.id, { name: category.name }).subscribe({
      next: () => this.fetch()
    });
  }

  delete(id: string): void {
    if (!confirm('Bu kategoriyi silmek istediğinize emin misiniz?')) return;
    this.service.delete(id).subscribe({
      next: () => this.fetch()
    });
  }
}
