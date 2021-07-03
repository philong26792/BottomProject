/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { KanbanByPoService } from './kanban-by-po.service';

describe('Service: KanbanByPo', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [KanbanByPoService]
    });
  });

  it('should ...', inject([KanbanByPoService], (service: KanbanByPoService) => {
    expect(service).toBeTruthy();
  }));
});
