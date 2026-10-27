import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { DashboardRoleGuard } from './dashboard-role.guard';
import { SessionService } from '../services/session.service';

describe('DashboardRoleGuard', () => {
  let guard: DashboardRoleGuard;
  let router: jasmine.SpyObj<Router>;
  let session: jasmine.SpyObj<SessionService>;

  beforeEach(() => {
    router = jasmine.createSpyObj<Router>('Router', ['createUrlTree']);
    session = jasmine.createSpyObj<SessionService>('SessionService', ['isAuthenticated', 'isAdmin']);
    router.createUrlTree.and.callFake((commands: any[]) => ({ commands } as any));
    TestBed.configureTestingModule({
      providers: [
        DashboardRoleGuard,
        { provide: Router, useValue: router },
        { provide: SessionService, useValue: session }
      ]
    });
    guard = TestBed.inject(DashboardRoleGuard);
  });

  it('redirects unauthenticated users to login', () => {
    session.isAuthenticated.and.returnValue(false);
    expect(guard.canActivate({ data: {} } as ActivatedRouteSnapshot)).toEqual({ commands: ['/enter-the-system'] });
  });

  it('redirects /dashboard to the administrator dashboard for admins', () => {
    session.isAuthenticated.and.returnValue(true);
    session.isAdmin.and.returnValue(true);
    expect(guard.canActivate({ data: { redirectByRole: true } } as ActivatedRouteSnapshot)).toEqual({ commands: ['/admin-dashboard'] });
  });

  it('redirects /dashboard to the user dashboard for regular users', () => {
    session.isAuthenticated.and.returnValue(true);
    session.isAdmin.and.returnValue(false);
    expect(guard.canActivate({ data: { redirectByRole: true } } as ActivatedRouteSnapshot)).toEqual({ commands: ['/user-dashboard'] });
  });

  it('prevents regular users from entering the admin dashboard', () => {
    session.isAuthenticated.and.returnValue(true);
    session.isAdmin.and.returnValue(false);
    expect(guard.canActivate({ data: { role: 'ADMIN' } } as ActivatedRouteSnapshot)).toEqual({ commands: ['/user-dashboard'] });
  });

  it('prevents admins from entering the user dashboard', () => {
    session.isAuthenticated.and.returnValue(true);
    session.isAdmin.and.returnValue(true);
    expect(guard.canActivate({ data: { role: 'USER' } } as ActivatedRouteSnapshot)).toEqual({ commands: ['/admin-dashboard'] });
  });

  it('allows an administrator to enter the admin dashboard', () => {
    session.isAuthenticated.and.returnValue(true);
    session.isAdmin.and.returnValue(true);
    expect(guard.canActivate({ data: { role: 'ADMIN' } } as ActivatedRouteSnapshot)).toBeTrue();
  });

  it('allows a regular user to enter the user dashboard', () => {
    session.isAuthenticated.and.returnValue(true);
    session.isAdmin.and.returnValue(false);
    expect(guard.canActivate({ data: { role: 'USER' } } as ActivatedRouteSnapshot)).toBeTrue();
  });
}