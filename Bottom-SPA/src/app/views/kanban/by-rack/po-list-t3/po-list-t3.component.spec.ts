/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { PoListT3Component } from './po-list-t3.component';

describe('PoListT3Component', () => {
  let component: PoListT3Component;
  let fixture: ComponentFixture<PoListT3Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PoListT3Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PoListT3Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
