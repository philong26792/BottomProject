/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { ListMailComponent } from './list-mail.component';

describe('ListMailComponent', () => {
  let component: ListMailComponent;
  let fixture: ComponentFixture<ListMailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListMailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListMailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
