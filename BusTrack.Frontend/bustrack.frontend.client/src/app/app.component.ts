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
    private applicationAvailabilityService:
      ApplicationAvailabilityService,

    private sessionService:
      SessionService
  ) {
  }


  ngOnInit(): void {

    this.sessionService.clearSession();


    this.applicationAvailabilityService
      .startMonitoring();


    blockSavePasswordRule();

  }

}
