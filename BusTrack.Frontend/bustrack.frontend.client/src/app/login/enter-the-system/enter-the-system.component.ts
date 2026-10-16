import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { NgForm } from '@angular/forms';

import {
  disableKeyboardShortcutsRule
} from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import {
  preventBackNavigationRule
} from '../rules/preventBackNavigationRules/preventBackNavigationRule';
import {
  preventForwardNavigationRule
} from '../rules/preventForwardNavigationRules/preventForwardNavigationRule';
import {
  blockSavePasswordRule
} from '../rules/blockSavePasswordRules/blockSavePasswordRule';
import { SessionService } from '../../services/session.service';
import { environment } from '../../../environments/environment';

interface LoginResponse {
  success: boolean;
  accessToken?: string;
  welcomeMessage?: string;
  message?: string;
}

@Component({
  selector: 'app-enter-the-system',
  templateUrl: './enter-the-system.component.html',
  styleUrls: ['./enter-the-system.component.css']
})
export class EnterTheSystemComponent implements OnInit {
  email = '';
  password = '';
  errorMessage = '';
  loginAttempts = 5;
  totalLoginAttempts = 5;

  constructor(
    private router: Router,
    private http: HttpClient,
    private sessionService: SessionService
  ) {}

  ngOnInit(): void {
    disableKeyboardShortcutsRule();
    preventBackNavigationRule();
    preventForwardNavigationRule();
    blockSavePasswordRule();
  }

  login(form: NgForm): void {
    if (!form.valid) {
      alert('Por favor, preencha todos os campos corretamente.');
      return;
    }

    const credentials = { email: this.email, password: this.password };

    this.http.post<LoginResponse>(
      `${environment.apiUrl}/AuthenticationControllerAPI/login`,
      credentials
    ).subscribe({
      next: (response) => {
        if (response.success && response.accessToken) {
          this.sessionService.startSession(response.accessToken);

          alert(
            response.welcomeMessage ??
            'Sucesso. Seja bem-vindo ao painel principal do Bus Track.'
          );

          void this.router.navigate(['/dashboard']);
          return;
        }

        this.sessionService.clearSession();
        alert('O servidor não retornou um token de acesso válido. Tente novamente.');
      },
      error: (err) => {
        this.sessionService.clearSession();

        if (this.loginAttempts > 0) {
          const message =
            err?.error?.message ??
            'E-mail, senha ou ambos não encontrados. Por favor, tente novamente.';

          alert(`${message}\n\nTentativas restantes: ${this.loginAttempts}`);
          this.loginAttempts--;
          return;
        }

        alert(
          'Você excedeu o número máximo de tentativas de login. Redirecionando para a tela de criar conta.'
        );
        void this.router.navigate(['/create-an-account']);
      }
    });
  }

  cancelar(): void {
    this.email = '';
    this.password = '';
    this.sessionService.clearSession();
    alert('Operação de login cancelada.');
    void this.router.navigate(['/main-screen']);
  }

  voltar(): void {
    this.sessionService.clearSession();
    void this.router.navigate(['/create-an-account']);
  }

  onSubmit(form: NgForm): void {
    this.login(form);
  }
}
