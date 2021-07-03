import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OtherSplitQrcodeDetailChildComponent } from './other-split-qrcode-detail-child.component';

describe('OtherSplitQrcodeDetailChildComponent', () => {
  let component: OtherSplitQrcodeDetailChildComponent;
  let fixture: ComponentFixture<OtherSplitQrcodeDetailChildComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OtherSplitQrcodeDetailChildComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OtherSplitQrcodeDetailChildComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
