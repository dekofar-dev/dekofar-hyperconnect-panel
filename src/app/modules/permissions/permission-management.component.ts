import { Component, OnInit } from '@angular/core';
import { PermissionsService, Permission } from './services/permissions.service';

// İzin yönetimi sayfası
@Component({
  selector: 'app-permission-management',
  templateUrl: './permission-management.component.html'
})
export class PermissionManagementComponent implements OnInit {
  permissions: Permission[] = [];
  selected: { [key: string]: boolean } = {};
  roleId: number | null = null;

  constructor(private service: PermissionsService) {}

  ngOnInit(): void {
    this.loadPermissions();
  }

  // İzinleri API'den çeker
  loadPermissions(): void {
    this.service.getAllPermissions().subscribe((res) => (this.permissions = res));
  }

  // Seçili izinleri role atar
  assign(): void {
    if (!this.roleId) {
      return;
    }
    const perms = this.permissions
      .filter((p) => this.selected[p.name])
      .map((p) => p.name);
    this.service.assignPermissionsToRole(this.roleId, perms).subscribe(() => {
      this.selected = {};
    });
  }
}
