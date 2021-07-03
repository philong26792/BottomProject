/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { NoByBatchInComponent } from './no-by-batch-in.component';

describe('NoByBatchInComponent', () => {
  let component: NoByBatchInComponent;
  let fixture: ComponentFixture<NoByBatchInComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NoByBatchInComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NoByBatchInComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
