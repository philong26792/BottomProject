import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintListMissingComponent } from './print-list-missing.component';

describe('PrintListMissingComponent', () => {
  let component: PrintListMissingComponent;
  let fixture: ComponentFixture<PrintListMissingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintListMissingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintListMissingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
