import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgCrudComponent } from './ag-crud.component';

describe('AgCrudComponent', () => {
  let component: AgCrudComponent;
  let fixture: ComponentFixture<AgCrudComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AgCrudComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AgCrudComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
