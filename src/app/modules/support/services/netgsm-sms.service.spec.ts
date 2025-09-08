import { TestBed } from '@angular/core/testing';

import { NetgsmSmsService } from './netgsm-sms.service';

describe('NetgsmSmsService', () => {
  let service: NetgsmSmsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NetgsmSmsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
