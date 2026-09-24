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

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title = 'BustTrack.Frontend';

  constructor(
    private applicationAvailabilityService:
      ApplicationAvailabilityService,

    private sessionService:
      SessionService
  ) {
  }

  ngOnInit(): void {

    /*
     * Toda nova inicialização do frontend começa
     * sem uma sessão anterior.
     *
     * Isso impede que uma sessão antiga seja
     * restaurada depois que o npm start foi
     * interrompido e iniciado novamente.
     */
    this.sessionService.clearSession();

    this.applicationAvailabilityService
      .startMonitoring();
  }
}
