import { TestBed } from '@angular/core/testing';

import { MaterialFormService } from './material-form.service';

describe('MaterialFormService', () => {
  let service: MaterialFormService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MaterialFormService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
