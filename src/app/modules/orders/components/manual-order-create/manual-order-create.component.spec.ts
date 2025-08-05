import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManualOrderCreateComponent } from './manual-order-create.component';

describe('ManualOrderCreateComponent', () => {
  let component: ManualOrderCreateComponent;
  let fixture: ComponentFixture<ManualOrderCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManualOrderCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManualOrderCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
