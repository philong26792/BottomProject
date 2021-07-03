import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MergeQrcodeMainComponent } from './merge-qrcode-main.component';

describe('MergeQrcodeMainComponent', () => {
  let component: MergeQrcodeMainComponent;
  let fixture: ComponentFixture<MergeQrcodeMainComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MergeQrcodeMainComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MergeQrcodeMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
