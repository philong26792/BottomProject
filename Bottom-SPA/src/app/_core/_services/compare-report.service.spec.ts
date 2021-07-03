/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { CompareReportService } from './compare-report.service';

describe('Service: CompareReport', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CompareReportService]
    });
  });

  it('should ...', inject([CompareReportService], (service: CompareReportService) => {
    expect(service).toBeTruthy();
  }));
});
