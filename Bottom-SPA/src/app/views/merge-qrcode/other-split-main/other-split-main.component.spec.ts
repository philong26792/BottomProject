import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OtherSplitMainComponent } from './other-split-main.component';

describe('OtherSplitMainComponent', () => {
  let component: OtherSplitMainComponent;
  let fixture: ComponentFixture<OtherSplitMainComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OtherSplitMainComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OtherSplitMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
