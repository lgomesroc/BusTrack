import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router, UrlTree } from '@angular/router';
import { SessionService } from '../services/session.service';

@Injectable({ providedIn: 'root' })
export class DashboardRoleGuard implements CanActivate {
  constructor(private session: SessionService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean | UrlTree {
    if (!this.session.isAuthenticated()) {
      return this.router.createUrlTree(['/enter-the-system']);
    }

    const isAdmin = this.session.isAdmin();
    if (route.data['redirectByRole'] === true) {
      return this.router.createUrlTree([isAdmin ? '/admin-dashboard' : '/user-dashboard']);
    }

    const expectedRole = route.data['role'] as 'ADMIN' | 'USER';
    if (expectedRole === 'ADMIN' && !isAdmin) {
      return this.router.createUrlTree(['/user-dashboard']);
    }
    if (expectedRole === 'USER' && isAdmin) {
      return this.router.createUrlTree(['/admin-dashboard']);
    }
    return true;
  }
}