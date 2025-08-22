import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExistingCustomerOrdersComponent } from './existing-customer-orders.component';

describe('ExistingCustomerOrdersComponent', () => {
  let component: ExistingCustomerOrdersComponent;
  let fixture: ComponentFixture<ExistingCustomerOrdersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ExistingCustomerOrdersComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExistingCustomerOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
