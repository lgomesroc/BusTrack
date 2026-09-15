import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';

import { EnterTheSystemComponent } from './enter-the-system.component';

describe('EnterTheSystemComponent', () => {
  let component: EnterTheSystemComponent;
  let fixture: ComponentFixture<EnterTheSystemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EnterTheSystemComponent],
      imports: [
        HttpClientTestingModule,
        FormsModule
      ]
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
