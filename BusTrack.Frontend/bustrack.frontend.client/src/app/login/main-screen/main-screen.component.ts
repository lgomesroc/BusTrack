import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { startInactivityTimerRule } from '../rules/inactivityTimerRules/inactivityTimerRule';
import { disableKeyboardShortcutsRule } from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';


@Component({
  selector: 'app-main-screen',
  templateUrl: './main-screen.component.html',
  styleUrls: ['./main-screen.component.css']
})

export class MainScreenComponent implements OnInit {
  inactivityTimer: any; // Adicionado
  INACTIVITY_TIMEOUT_MS = 1200000; // Adicionado

  constructor(private router: Router) { }

  entrarNoSistema() {
    this.router.navigate(['/enter-the-system']);
  }

  criarConta() {
    this.router.navigate(['/create-an-account']);
  }

  ngOnInit(): void {
    // Implementação de regras comuns a todas as telas
    disableKeyboardShortcutsRule();
    startInactivityTimerRule(this.INACTIVITY_TIMEOUT_MS);
  }
}
