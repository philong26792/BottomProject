import { TestBed } from '@angular/core/testing';

import { SettingMailService } from './setting-mail.service';

describe('SettingMailService', () => {
  let service: SettingMailService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SettingMailService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
