import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderItemsSummaryComponent } from './order-items-summary.component';

describe('OrderItemsSummaryComponent', () => {
  let component: OrderItemsSummaryComponent;
  let fixture: ComponentFixture<OrderItemsSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderItemsSummaryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderItemsSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
