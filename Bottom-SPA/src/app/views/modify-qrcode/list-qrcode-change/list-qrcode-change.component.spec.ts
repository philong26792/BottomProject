import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListQrcodeChangeComponent } from './list-qrcode-change.component';

describe('ListQrcodeChangeComponent', () => {
  let component: ListQrcodeChangeComponent;
  let fixture: ComponentFixture<ListQrcodeChangeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListQrcodeChangeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListQrcodeChangeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
