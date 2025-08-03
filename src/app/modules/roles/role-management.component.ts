import { Component, OnInit } from '@angular/core';
import { RolesService, Role } from './services/roles.service';

// Rol yönetimi sayfası
@Component({
  selector: 'app-role-management',
  templateUrl: './role-management.component.html'
})
export class RoleManagementComponent implements OnInit {
  roles: Role[] = [];
  newRole = '';
  modalVisible = false;
  selectedRoleName = '';

  constructor(private service: RolesService) {}

  ngOnInit(): void {
    this.loadRoles();
  }

  // Rolleri API'den çeker
  loadRoles(): void {
    this.service.getAllRoles().subscribe((res) => (this.roles = res));
  }

  // Yeni rol ekler
  addRole(): void {
    if (!this.newRole) {
      return;
    }
    this.service.createRole(this.newRole).subscribe(() => {
      this.newRole = '';
      this.loadRoles();
    });
  }

  // Rolü siler
  deleteRole(id: number): void {
    this.service.deleteRole(id).subscribe(() => this.loadRoles());
  }

  // Modalı açar
  openAssignModal(role: Role): void {
    this.selectedRoleName = role.name;
    this.modalVisible = true;
  }

  // Kullanıcıya rol atama işlemi
  handleAssign(userId: string): void {
    this.service.assignRoleToUser(userId, this.selectedRoleName).subscribe(() => {
      this.modalVisible = false;
    });
  }
}
