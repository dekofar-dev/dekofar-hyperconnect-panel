import { TestBed } from '@angular/core/testing';

import { MediaDownloaderService } from './media-downloader.service';

describe('MediaDownloaderService', () => {
  let service: MediaDownloaderService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MediaDownloaderService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
