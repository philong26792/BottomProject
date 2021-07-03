import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReceiveMainComponent } from './receive-main.component';

describe('ReceiveMainComponent', () => {
  let component: ReceiveMainComponent;
  let fixture: ComponentFixture<ReceiveMainComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReceiveMainComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReceiveMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
