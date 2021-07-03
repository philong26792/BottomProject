/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { KanbanByrackService } from './kanban-byrack.service';

describe('Service: KanbanByrack', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [KanbanByrackService]
    });
  });

  it('should ...', inject([KanbanByrackService], (service: KanbanByrackService) => {
    expect(service).toBeTruthy();
  }));
});
