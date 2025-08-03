import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ColDef, GridReadyEvent } from 'ag-grid-community';

@Component({
  selector: 'app-ag-crud',
  templateUrl: './ag-crud.component.html',
  styleUrls: ['./ag-crud.component.scss']
})
export class AgCrudComponent {
  @Input() rowData: any[] = [];
  @Input() columnDefs: ColDef[] = [];

  @Output() editEvent = new EventEmitter<any>();
  @Output() deleteEvent = new EventEmitter<any>();
  @Output() createEvent = new EventEmitter<void>();

  defaultColDef: ColDef = {
    sortable: true,
    filter: true,
    resizable: true
  };

  onGridReady(params: GridReadyEvent) {
    params.api.sizeColumnsToFit();
  }

  onEdit(row: any) {
    this.editEvent.emit(row);
  }

  onDelete(row: any) {
    this.deleteEvent.emit(row);
  }

  onCreate() {
    this.createEvent.emit();
  }
}
