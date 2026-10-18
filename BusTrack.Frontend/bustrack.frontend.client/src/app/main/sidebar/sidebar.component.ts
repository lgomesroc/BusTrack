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
  nomeUsuario = 'Usuário autenticado';
  cpfUsuario = '';
  readonly INACTIVITY_TIMEOUT_MS = 1200000;

  ngOnInit(): void {
    BlockCopyRule();
    disableKeyboardShortcutsRule();
    preventBackNavigationRule();
    preventForwardNavigationRule();
    startInactivityTimerRule(this.INACTIVITY_TIMEOUT_MS);
    clearInactivityTimerRule();
  }
}