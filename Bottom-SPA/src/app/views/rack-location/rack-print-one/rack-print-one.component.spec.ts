/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { RackPrintOneComponent } from './rack-print-one.component';

describe('RackPrintOneComponent', () => {
  let component: RackPrintOneComponent;
  let fixture: ComponentFixture<RackPrintOneComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RackPrintOneComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RackPrintOneComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
