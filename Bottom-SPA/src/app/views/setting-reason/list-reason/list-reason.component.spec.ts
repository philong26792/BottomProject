import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListReasonComponent } from './list-reason.component';

describe('ListReasonComponent', () => {
  let component: ListReasonComponent;
  let fixture: ComponentFixture<ListReasonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListReasonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListReasonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
