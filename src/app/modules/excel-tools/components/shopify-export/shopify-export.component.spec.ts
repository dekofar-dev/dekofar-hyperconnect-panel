import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShopifyExportComponent } from './shopify-export.component';

describe('ShopifyExportComponent', () => {
  let component: ShopifyExportComponent;
  let fixture: ComponentFixture<ShopifyExportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShopifyExportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShopifyExportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
