import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SplitQrcodeDetailChildComponent } from './split-qrcode-detail-child.component';

describe('SplitQrcodeDetailChildComponent', () => {
  let component: SplitQrcodeDetailChildComponent;
  let fixture: ComponentFixture<SplitQrcodeDetailChildComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SplitQrcodeDetailChildComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SplitQrcodeDetailChildComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
