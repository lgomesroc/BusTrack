import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthenticationGuard } from './authentication.guard';
import { SessionService } from '../services/session.service';

describe('AuthenticationGuard', () => {
  let guard: AuthenticationGuard;
  let sessionService: jasmine.SpyObj<SessionService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    sessionService = jasmine.createSpyObj<SessionService>(
      'SessionService',
      ['isAuthenticated']
    );
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        AuthenticationGuard,
        { provide: SessionService, useValue: sessionService },
        { provide: Router, useValue: router }
      ]
    });

    guard = TestBed.inject(AuthenticationGuard);
  });

  it('allows navigation when the user is authenticated', () => {
    sessionService.isAuthenticated.and.returnValue(true);

    expect(guard.canActivate()).toBeTrue();
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('redirects unauthenticated users to the sign-in page', () => {
    sessionService.isAuthenticated.and.returnValue(false);

    expect(guard.canActivate()).toBeFalse();
    expect(router.navigate).toHaveBeenCalledWith(
      ['/enter-the-system'],
      { replaceUrl: true }
    );
  });
});
