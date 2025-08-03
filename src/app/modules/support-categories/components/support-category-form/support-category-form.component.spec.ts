import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SupportCategoryFormComponent } from './support-category-form.component';

describe('SupportCategoryFormComponent', () => {
  let component: SupportCategoryFormComponent;
  let fixture: ComponentFixture<SupportCategoryFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SupportCategoryFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SupportCategoryFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
