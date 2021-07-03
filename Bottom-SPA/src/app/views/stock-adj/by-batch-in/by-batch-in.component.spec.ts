/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { ByBatchInComponent } from './by-batch-in.component';

describe('ByBatchInComponent', () => {
  let component: ByBatchInComponent;
  let fixture: ComponentFixture<ByBatchInComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ByBatchInComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ByBatchInComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
