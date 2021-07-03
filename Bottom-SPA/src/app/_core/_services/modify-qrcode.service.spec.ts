/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { ModifyQrcodeService } from './modify-qrcode.service';

describe('Service: ModifyQrcode', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ModifyQrcodeService]
    });
  });

  it('should ...', inject([ModifyQrcodeService], (service: ModifyQrcodeService) => {
    expect(service).toBeTruthy();
  }));
});
