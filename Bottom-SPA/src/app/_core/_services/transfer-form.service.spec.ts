/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { TransferFormService } from './transfer-form.service';

describe('Service: TransferForm', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TransferFormService]
    });
  });

  it('should ...', inject([TransferFormService], (service: TransferFormService) => {
    expect(service).toBeTruthy();
  }));
});
