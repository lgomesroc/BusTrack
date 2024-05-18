// sidebar.component.ts
import { Component, OnInit } from '@angular/core';
import { BlockCopyRule } from '../../login/rules/blockCopyRules/blockCopyRule';
import { disableKeyboardShortcutsRule } from '../../login/rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import { preventBackNavigationRule } from '../../login/rules/preventBackNavigationRules/preventBackNavigationRule';
import { preventForwardNavigationRule } from '../../login/rules/preventForwardNavigationRules/preventForwardNavigationRule';
import { startInactivityTimerRule, clearInactivityTimerRule } from '../../login/rules/inactivityTimerRules/inactivityTimerRule';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  nomeUsuario: string = "João Silva";
  cpfUsuario: string = "123.456.789-00";
  INACTIVITY_TIMEOUT_MS = 1200000; // Tempo de inatividade em milissegundos

  constructor() { }

  ngOnInit(): void {
    BlockCopyRule();
    disableKeyboardShortcutsRule();
    preventBackNavigationRule();
    preventForwardNavigationRule();
    startInactivityTimerRule(this.INACTIVITY_TIMEOUT_MS);
    clearInactivityTimerRule();
  }
}
