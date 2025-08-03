import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RolesRoutingModule } from './roles-routing.module';
import { RoleManagementComponent } from './role-management.component';
import { AssignRoleModalComponent } from './components/assign-role-modal/assign-role-modal.component';

// Rol yönetim modülü
@NgModule({
  declarations: [RoleManagementComponent, AssignRoleModalComponent],
  imports: [CommonModule, FormsModule, RolesRoutingModule],
})
export class RolesModule {}
