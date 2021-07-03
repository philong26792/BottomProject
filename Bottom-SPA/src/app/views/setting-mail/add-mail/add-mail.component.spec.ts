/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { AddMailComponent } from './add-mail.component';

describe('AddMailComponent', () => {
  let component: AddMailComponent;
  let fixture: ComponentFixture<AddMailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddMailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddMailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
