import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NoByBatchOutComponent } from './no-by-batch-out.component';

describe('NoByBatchOutComponent', () => {
  let component: NoByBatchOutComponent;
  let fixture: ComponentFixture<NoByBatchOutComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NoByBatchOutComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NoByBatchOutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
