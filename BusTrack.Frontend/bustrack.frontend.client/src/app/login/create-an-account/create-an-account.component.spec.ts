import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateAnAccountComponent } from '../create-an-account/create-an-account.component';

describe('CreateAnAccountComponent', () => {
  let component: CreateAnAccountComponent;
  let fixture: ComponentFixture<CreateAnAccountComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CreateAnAccountComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CreateAnAccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
