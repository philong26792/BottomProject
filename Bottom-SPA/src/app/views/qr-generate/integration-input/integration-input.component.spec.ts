import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IntegrationInputComponent } from './integration-input.component';

describe('IntegrationInputComponent', () => {
  let component: IntegrationInputComponent;
  let fixture: ComponentFixture<IntegrationInputComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IntegrationInputComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IntegrationInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
