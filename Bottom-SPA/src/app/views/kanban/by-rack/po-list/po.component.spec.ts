/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { PoComponent } from './po.component';

describe('PoComponent', () => {
  let component: PoComponent;
  let fixture: ComponentFixture<PoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
