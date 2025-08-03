import { Component, EventEmitter, Input, Output } from '@angular/core';

// Kullanıcıya rol atamak için açılan modal bileşeni
@Component({
  selector: 'app-assign-role-modal',
  templateUrl: './assign-role-modal.component.html',
})
export class AssignRoleModalComponent {
  @Input() visible = false; // Modal görünürlüğü
  @Input() roleName = '';
  @Output() close = new EventEmitter<void>();
  @Output() assign = new EventEmitter<string>();

  userId = '';

  // Ata butonuna basıldığında çalışır
  submit(): void {
    this.assign.emit(this.userId);
  }
}
