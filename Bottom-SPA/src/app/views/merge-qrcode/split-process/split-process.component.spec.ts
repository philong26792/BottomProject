import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SplitProcessComponent } from './split-process.component';

describe('SplitProcessComponent', () => {
  let component: SplitProcessComponent;
  let fixture: ComponentFixture<SplitProcessComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SplitProcessComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SplitProcessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
