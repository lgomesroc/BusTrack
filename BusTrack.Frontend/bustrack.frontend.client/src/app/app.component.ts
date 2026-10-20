import {
  Component,
  OnInit
} from '@angular/core';

import {
  ApplicationAvailabilityService
} from './services/application-availability.service';

import {
  SessionService
} from './services/session.service';

import {
  blockSavePasswordRule
} from './login/rules/blockSavePasswordRules/blockSavePasswordRule';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title = 'BustTrack.Frontend';

  constructor(
    private readonly applicationAvailabilityService:
      ApplicationAvailabilityService,

    private readonly sessionService:
      SessionService
  ) {
  }

  ngOnInit(): void {
    // Mantém o identificador deste perfil de navegador.
    // Não apaga o token: F5 deve preservar o login.
    this.sessionService.getBrowserId();

    this.applicationAvailabilityService.startMonitoring();

    blockSavePasswordRule();
  }
}
