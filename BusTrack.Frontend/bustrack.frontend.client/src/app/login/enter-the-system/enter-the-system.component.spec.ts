import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EnterTheSystemComponent } from './enter-the-system.component';

describe('EnterTheSystemComponent', () => {
  let component: EnterTheSystemComponent;
  let fixture: ComponentFixture<EnterTheSystemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EnterTheSystemComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EnterTheSystemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
