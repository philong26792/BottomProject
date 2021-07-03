import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailQrcodeComponent } from './detail-qrcode.component';

describe('DetailQrcodeComponent', () => {
  let component: DetailQrcodeComponent;
  let fixture: ComponentFixture<DetailQrcodeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DetailQrcodeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailQrcodeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
