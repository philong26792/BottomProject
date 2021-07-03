/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { HpUploadService } from './hp-upload.service';

describe('Service: HpUpload', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [HpUploadService]
    });
  });

  it('should ...', inject([HpUploadService], (service: HpUploadService) => {
    expect(service).toBeTruthy();
  }));
});
