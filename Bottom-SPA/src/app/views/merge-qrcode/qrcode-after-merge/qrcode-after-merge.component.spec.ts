import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QrcodeAfterMergeComponent } from './qrcode-after-merge.component';

describe('QrcodeAfterMergeComponent', () => {
  let component: QrcodeAfterMergeComponent;
  let fixture: ComponentFixture<QrcodeAfterMergeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ QrcodeAfterMergeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QrcodeAfterMergeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
