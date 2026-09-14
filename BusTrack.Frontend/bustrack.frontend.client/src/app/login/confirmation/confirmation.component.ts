import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { startInactivityTimerRule } from '../rules/inactivityTimerRules/inactivityTimerRule';
import { disableKeyboardShortcutsRule } from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import { preventBackNavigationRule } from '../rules/preventBackNavigationRules/preventBackNavigationRule';
import { preventForwardNavigationRule } from '../rules/preventForwardNavigationRules/preventForwardNavigationRule';

@Component({
  selector: 'app-confirmation',
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.css']
})
export class ConfirmationComponent implements OnInit {
  fullName: string = '';
  cpf: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  errorMessage: string = '';
  loginAttempts: number = 5;
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

  confirmarCadastro(): void {
    this.router.navigate(['/conclusion']);
  }
}
