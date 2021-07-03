import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OtherSplitQrcodeDetailComponent } from './other-split-qrcode-detail.component';

describe('OtherSplitQrcodeDetailComponent', () => {
  let component: OtherSplitQrcodeDetailComponent;
  let fixture: ComponentFixture<OtherSplitQrcodeDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OtherSplitQrcodeDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OtherSplitQrcodeDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
