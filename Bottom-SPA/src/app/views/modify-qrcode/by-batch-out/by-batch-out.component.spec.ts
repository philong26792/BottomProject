import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ByBatchOutComponent } from './by-batch-out.component';

describe('ByBatchOutComponent', () => {
  let component: ByBatchOutComponent;
  let fixture: ComponentFixture<ByBatchOutComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ByBatchOutComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ByBatchOutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
