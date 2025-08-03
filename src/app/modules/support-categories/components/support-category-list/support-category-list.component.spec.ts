import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SupportCategoryListComponent } from './support-category-list.component';

describe('SupportCategoryListComponent', () => {
  let component: SupportCategoryListComponent;
  let fixture: ComponentFixture<SupportCategoryListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SupportCategoryListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SupportCategoryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
