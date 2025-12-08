import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MediaDownloaderComponent } from './media-downloader.component';

describe('MediaDownloaderComponent', () => {
  let component: MediaDownloaderComponent;
  let fixture: ComponentFixture<MediaDownloaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MediaDownloaderComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MediaDownloaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
