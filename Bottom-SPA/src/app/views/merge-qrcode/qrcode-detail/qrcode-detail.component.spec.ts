import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QrcodeDetailComponent } from './qrcode-detail.component';

describe('QrcodeDetailComponent', () => {
  let component: QrcodeDetailComponent;
  let fixture: ComponentFixture<QrcodeDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ QrcodeDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QrcodeDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
