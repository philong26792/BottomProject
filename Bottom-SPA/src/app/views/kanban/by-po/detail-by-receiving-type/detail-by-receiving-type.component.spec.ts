import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailByReceivingTypeComponent } from './detail-by-receiving-type.component';

describe('DetailByReceivingTypeComponent', () => {
  let component: DetailByReceivingTypeComponent;
  let fixture: ComponentFixture<DetailByReceivingTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DetailByReceivingTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailByReceivingTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
