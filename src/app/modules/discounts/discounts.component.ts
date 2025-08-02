import { Component, OnInit } from '@angular/core';
import { DiscountService, Discount } from './services/discount.service';

@Component({
  selector: 'app-discounts',
  templateUrl: './discounts.component.html'
})
export class DiscountsComponent implements OnInit {
  discounts: Discount[] = [];
  loading = false;

  constructor(private service: DiscountService) {}

  ngOnInit(): void {
    this.fetch();
  }

  fetch(): void {
    this.loading = true;
    this.service.getAll().subscribe({
      next: (res) => {
        this.discounts = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
