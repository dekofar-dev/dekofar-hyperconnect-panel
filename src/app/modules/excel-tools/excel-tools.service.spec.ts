import { TestBed } from '@angular/core/testing';

import { ExcelToolsService } from './excel-tools.service';

describe('ExcelToolsService', () => {
  let service: ExcelToolsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ExcelToolsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
