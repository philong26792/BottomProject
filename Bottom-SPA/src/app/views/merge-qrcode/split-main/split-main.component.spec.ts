import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SplitMainComponent } from './split-main.component';

describe('SplitMainComponent', () => {
  let component: SplitMainComponent;
  let fixture: ComponentFixture<SplitMainComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SplitMainComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SplitMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
