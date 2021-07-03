import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CompareReportComponent } from './compare-report.component';

describe('CompareReportComponent', () => {
  let component: CompareReportComponent;
  let fixture: ComponentFixture<CompareReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CompareReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CompareReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
