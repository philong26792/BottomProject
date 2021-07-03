import { TestBed } from '@angular/core/testing';

import { SettingReasonService } from './setting-reason.service';

describe('SettingReasonService', () => {
  let service: SettingReasonService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SettingReasonService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
