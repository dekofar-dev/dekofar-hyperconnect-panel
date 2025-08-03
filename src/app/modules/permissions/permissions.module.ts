import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PermissionsRoutingModule } from './permissions-routing.module';
import { PermissionManagementComponent } from './permission-management.component';

// İzin yönetim modülü
@NgModule({
  declarations: [PermissionManagementComponent],
  imports: [CommonModule, FormsModule, PermissionsRoutingModule],
})
export class PermissionsModule {}
