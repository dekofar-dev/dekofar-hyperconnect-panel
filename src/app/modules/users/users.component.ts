import { Component, OnInit } from '@angular/core';
import { UsersService, AppUser } from './services/users.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html'
})
export class UsersComponent implements OnInit {
  users: AppUser[] = [];
  loading = false;

  constructor(private service: UsersService) {}

  ngOnInit(): void {
    this.fetch();
  }

  fetch(): void {
    this.loading = true;
    this.service.getAll().subscribe({
      next: (res) => {
        this.users = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  getInitials(name: string): string {
    if (!name) {
      return '';
    }
    return name
      .split(' ')
      .filter((n) => n)
      .map((n) => n[0])
      .join('')
      .toUpperCase();
  }
}
