import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { startInactivityTimerRule } from '../rules/inactivityTimerRules/inactivityTimerRule';
import { disableKeyboardShortcutsRule } from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import { preventBackNavigationRule } from '../rules/preventBackNavigationRules/preventBackNavigationRule';
import { preventForwardNavigationRule } from '../rules/preventForwardNavigationRules/preventForwardNavigationRule';

@Component({
  selector: 'app-concluded',
  templateUrl: './concluded.component.html',
  styleUrls: ['./concluded.component.css']
})
export class ConcludedComponent implements OnInit {
  inactivityTimer: any;
  INACTIVITY_TIMEOUT_MS = 1200000;

  constructor(private router: Router) {
  }

  ngOnInit(): void {
    disableKeyboardShortcutsRule();
    startInactivityTimerRule(this.INACTIVITY_TIMEOUT_MS);
    preventBackNavigationRule();
    preventForwardNavigationRule();
  }

  redirectToLogin(): void {
    this.router.navigate(['/enter-the-system']);
  }
}
