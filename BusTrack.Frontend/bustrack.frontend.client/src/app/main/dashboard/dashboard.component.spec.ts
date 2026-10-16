import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, RouterTestingModule } from '@angular/router/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';

import { DashboardComponent } from './dashboard.component';
import { SessionService } from '../../services/session.service';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
  let sessionService: SessionService;
  let router: Router;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        DashboardComponent
      ],
      imports: [
        RouterTestingModule
      ],
      providers: [
        SessionService
      ],
      schemas: [
        NO_ERRORS_SCHEMA
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    sessionService = TestBed.inject(SessionService);
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  afterEach(() => {
    sessionService.clearSession();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should clear the session and redirect to login when logging out', () => {
    sessionService.startSession();
    const navigateSpy = spyOn(router, 'navigate').and.returnValue(Promise.resolve(true));

    component.sair();

    expect(sessionService.isAuthenticated()).toBeFalse();
    expect(navigateSpy).toHaveBeenCalledWith(
      ['/enter-the-system'],
      { replaceUrl: true }
    );
  });
});
