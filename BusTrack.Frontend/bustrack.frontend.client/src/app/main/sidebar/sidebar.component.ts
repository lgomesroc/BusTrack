import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BlockCopyRule } from '../../login/rules/blockCopyRules/blockCopyRule';
import { disableKeyboardShortcutsRule } from '../../login/rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import { preventBackNavigationRule } from '../../login/rules/preventBackNavigationRules/preventBackNavigationRule';
import { preventForwardNavigationRule } from '../../login/rules/preventForwardNavigationRules/preventForwardNavigationRule';
import { startInactivityTimerRule, clearInactivityTimerRule } from '../../login/rules/inactivityTimerRules/inactivityTimerRule';
import { SessionService } from '../../services/session.service';
import { environment } from '../../../environments/environment';

interface CurrentUser {
  fullName?: string;
  CPF?: string;
  Email?: string;
  Login?: string;
}

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  nomeUsuario = 'Usuário';
  cpfUsuario = '';
  INACTIVITY_TIMEOUT_MS = 1200000;

  constructor(private http: HttpClient, private sessionService: SessionService) { }

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
      const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
      const userId = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || payload.sub;
      if (userId) {
        this.http.get<CurrentUser>(`${environment.apiUrl}/Authentication/${userId}`).subscribe({
          next: user => {
            this.nomeUsuario = user.fullName || user.Login || user.Email || 'Usuário';
            this.cpfUsuario = user.CPF || '';
          },
          error: () => { this.nomeUsuario = 'Usuário autenticado'; }
        });
      }
    } catch {
      this.nomeUsuario = 'Usuário autenticado';
    }
  }
}