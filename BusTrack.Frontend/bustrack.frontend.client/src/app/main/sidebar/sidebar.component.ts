import { Component, OnInit } from '@angular/core';
import { BlockCopyRule } from '../../login/rules/blockCopyRules/blockCopyRule';
import { disableKeyboardShortcutsRule } from '../../login/rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import { preventBackNavigationRule } from '../../login/rules/preventBackNavigationRules/preventBackNavigationRule';
import { preventForwardNavigationRule } from '../../login/rules/preventForwardNavigationRules/preventForwardNavigationRule';
import { startInactivityTimerRule, clearInactivityTimerRule } from '../../login/rules/inactivityTimerRules/inactivityTimerRule';
import { SessionService } from '../../services/session.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  nomeUsuario = 'Usuário autenticado';
  emailUsuario = '';
  perfilUsuario = 'USER';
  readonly INACTIVITY_TIMEOUT_MS = 1200000;

  constructor(private sessionService: SessionService) {}

  ngOnInit(): void {
    BlockCopyRule();
    disableKeyboardShortcutsRule();
    preventBackNavigationRule();
    preventForwardNavigationRule();
    startInactivityTimerRule(this.INACTIVITY_TIMEOUT_MS);
    clearInactivityTimerRule();

    const token = this.sessionService.getAccessToken();
    if (!token) return;
    try {
      const part = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
      const payload = JSON.parse(atob(part.padEnd(part.length + ((4 - part.length % 4) % 4), '='))) as Record<string, unknown>;
      const name = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ?? payload.name;
      const email = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? payload.email;
      const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? payload.role;
      if (typeof name === 'string' && name.trim()) this.nomeUsuario = name;
      else if (typeof email === 'string' && email.trim()) this.nomeUsuario = email;
      if (typeof email === 'string') this.emailUsuario = email;
      if (typeof role === 'string' && role.trim()) this.perfilUsuario = role;
    } catch {
      this.nomeUsuario = 'Usuário autenticado';
    }
  }
}