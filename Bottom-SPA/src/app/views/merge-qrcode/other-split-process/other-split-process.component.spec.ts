import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OtherSplitProcessComponent } from './other-split-process.component';

describe('OtherSplitProcessComponent', () => {
  let component: OtherSplitProcessComponent;
  let fixture: ComponentFixture<OtherSplitProcessComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OtherSplitProcessComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OtherSplitProcessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
