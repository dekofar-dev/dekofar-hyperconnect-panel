import { Component, OnInit } from '@angular/core';
import { CommissionsService, CommissionSummary } from './services/commissions.service';

@Component({
  selector: 'app-commissions',
  templateUrl: './commissions.component.html'
})
export class CommissionsComponent implements OnInit {
  summary: CommissionSummary[] = [];
  loading = false;

  constructor(private service: CommissionsService) {}

  ngOnInit(): void {
    this.fetch();
  }

  fetch(): void {
    this.loading = true;
    this.service.getMonthlySummary().subscribe({
      next: (res) => {
        this.summary = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}

