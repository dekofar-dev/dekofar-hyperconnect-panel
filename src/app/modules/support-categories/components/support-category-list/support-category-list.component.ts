import { Component, OnInit } from '@angular/core';
import { ColDef } from 'ag-grid-community';
import { SupportCategoryService } from '../../services/support-category.service';
import { SupportCategory } from '../../models/support-category.model';
import { Observable } from 'rxjs/internal/Observable';

@Component({
  selector: 'app-support-category-list',
  templateUrl: './support-category-list.component.html'
})
export class SupportCategoryListComponent implements OnInit {
  rowData: SupportCategory[] = [];
  columnDefs: ColDef[] = [];

  selectedCategory: Partial<SupportCategory> = {};
  isEdit = false;

  constructor(private service: SupportCategoryService) {}

  ngOnInit(): void {
    this.loadCategories();

    this.columnDefs = [
      { field: 'id', headerName: 'ID' },
      { field: 'name', headerName: 'Kategori Adı' },
      {
        headerName: 'Roller',
        field: 'roles',
        valueFormatter: (params) =>
          Array.isArray(params.value)
            ? params.value.map((r: any) => r.roleName).join(', ')
            : '-'
      },
      {
        headerName: 'Oluşturulma',
        field: 'createdAt',
        valueFormatter: (params) =>
          new Date(params.value).toLocaleString()
      },
      {
        headerName: 'İşlemler',
        cellRenderer: (params: any) => `
          <button class="btn btn-sm btn-primary edit-btn">Düzenle</button>
          <button class="btn btn-sm btn-danger delete-btn ms-2">Sil</button>
        `,
        onCellClicked: (event: any) => {
          if (event.event.target.classList.contains('edit-btn')) {
            this.onEdit(event.data);
          } else if (event.event.target.classList.contains('delete-btn')) {
            this.onDelete(event.data);
          }
        },
        sortable: false,
        filter: false
      }
    ];
  }

  loadCategories(): void {
    this.service.getAll().subscribe((data) => {
      this.rowData = data;
    });
  }

  onCreate(): void {
    this.selectedCategory = {};
    this.isEdit = false;
    // Modal aç
  }

  onEdit(category: SupportCategory): void {
    this.selectedCategory = { ...category };
    this.isEdit = true;
    // Modal aç
  }

  onDelete(category: SupportCategory): void {
    this.service.delete(category.id!).subscribe(() => {
      this.loadCategories();
    });
  }

  onSave(): void {
    const name = this.selectedCategory.name?.trim();
    if (!name) return;

const action$ = (this.isEdit
  ? this.service.update(this.selectedCategory.id!, { name })
  : this.service.create({ name })) as Observable<any>;

action$.subscribe(() => this.loadCategories());
  }
}
