import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConcludedComponent } from './concluded.component';

describe('ConcludedComponent', () => {
  let component: ConcludedComponent;
  let fixture: ComponentFixture<ConcludedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ConcludedComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ConcludedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
