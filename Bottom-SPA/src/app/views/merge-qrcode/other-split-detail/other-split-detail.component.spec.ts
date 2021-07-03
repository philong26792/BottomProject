import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OtherSplitDetailComponent } from './other-split-detail.component';

describe('OtherSplitDetailComponent', () => {
  let component: OtherSplitDetailComponent;
  let fixture: ComponentFixture<OtherSplitDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OtherSplitDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OtherSplitDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
