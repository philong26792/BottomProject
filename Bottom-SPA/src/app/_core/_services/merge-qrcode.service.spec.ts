/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { MergeQrcodeService } from './merge-qrcode.service';

describe('Service: MergeQrcode', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MergeQrcodeService]
    });
  });

  it('should ...', inject([MergeQrcodeService], (service: MergeQrcodeService) => {
    expect(service).toBeTruthy();
  }));
});
