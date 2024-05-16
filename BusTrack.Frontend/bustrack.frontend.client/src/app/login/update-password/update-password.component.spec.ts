import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdatePasswordComponent } from './update-password.component';

describe('UpdatePasswordComponentComponent', () => {
  let component: UpdatePasswordComponent;
  let fixture: ComponentFixture<UpdatePasswordComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UpdatePasswordComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UpdatePasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
