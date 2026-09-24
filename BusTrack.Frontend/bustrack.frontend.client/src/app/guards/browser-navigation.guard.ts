import {
  Injectable
} from '@angular/core';

import {
  CanActivate,
  Router
} from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class BrowserNavigationGuard
  implements CanActivate {

  constructor(
    private router: Router
  ) {
  }

  canActivate(): boolean {

    const navigation =
      this.router.getCurrentNavigation();

    if (
      navigation?.trigger === 'popstate'
    ) {

      alert(
        'A navegação para trás e para frente pelo navegador está desativada.'
      );

      return false;
    }

    return true;
  }
}
