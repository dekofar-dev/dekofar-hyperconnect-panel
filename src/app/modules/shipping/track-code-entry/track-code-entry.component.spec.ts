import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrackCodeEntryComponent } from './track-code-entry.component';

describe('TrackCodeEntryComponent', () => {
  let component: TrackCodeEntryComponent;
  let fixture: ComponentFixture<TrackCodeEntryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrackCodeEntryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrackCodeEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
