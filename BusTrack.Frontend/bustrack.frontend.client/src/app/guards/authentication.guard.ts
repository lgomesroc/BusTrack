import { Injectable } from '@angular/core';
import {
  CanActivate,
  Router
} from '@angular/router';

import { SessionService } from '../services/session.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationGuard
  implements CanActivate {

  constructor(
    private sessionService: SessionService,
    private router: Router
  ) {
  }

  canActivate(): boolean {
    if (
      this.sessionService.isAuthenticated()
    ) {
      return true;
    }

    this.router.navigate(
      ['/enter-the-system'],
      {
        replaceUrl: true
      }
    );

    return false;
  }
}
