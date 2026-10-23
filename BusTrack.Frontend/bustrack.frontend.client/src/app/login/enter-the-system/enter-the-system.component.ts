
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
  code?: string;
  passwordExpirationWarning?: boolean;
  passwordDaysUntilExpiration?: number;
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
    private readonly router: Router,
    private readonly http: HttpClient,
    private readonly sessionService: SessionService
  ) {}

  ngOnInit(): void {
    disableKeyboardShortcutsRule();
    preventBackNavigationRule();
    preventForwardNavigationRule();
    blockSavePasswordRule();

    this.sessionService.getBrowserId();
  }

  login(form: NgForm): void {
    if (!form.valid) {
      this.errorMessage = 'Informe seu e-mail e sua senha.';
      return;
    }

    this.errorMessage = '';

    this.http.post<LoginResponse>(
      `${environment.apiUrl}/AuthenticationControllerAPI/login`,
      {
        email: this.email,
        password: this.password
      }
    ).subscribe({
      next: (response) => {
        if (response.success && response.accessToken) {
          this.sessionService.startSession(response.accessToken);

          if (response.passwordExpirationWarning) {
            const days = response.passwordDaysUntilExpiration ?? 0;

            alert(
              days > 0
                ? `Atenção: sua senha expirará em ${days} dia(s). Troque-a em breve.`
                : `Atenção: sua senha expirou. Troque-a assim que possível.`
            );
          }

          alert(
            response.welcomeMessage ??
            'Sucesso. Seja bem-vindo ao painel principal do Bus Track.'
          );

          void this.router.navigate(['/dashboard']);
          return;
        }

        this.sessionService.clearSession();
        this.errorMessage =
          'O servidor não retornou um token de acesso válido.';
      },
      error: (error) => {
        this.sessionService.clearSession();

        if (error?.error?.code === 'PASSWORD_EXPIRED') {
          this.errorMessage =
            'Sua senha expirou. Use “Trocar senha” para criar uma nova.';
          return;
        }

        if (this.loginAttempts > 0) {
          this.errorMessage =
            error?.error?.message ??
            'E-mail ou senha incorretos.';

          this.loginAttempts--;
          return;
        }

        this.errorMessage =
          'Você excedeu o número de tentativas desta tela. Tente novamente mais tarde.';
      }
    });
  }

  cancelar(): void {
    this.email = '';
    this.password = '';
    this.sessionService.clearSession();

    void this.router.navigate(['/main-screen']);
  }

  voltar(): void {
    this.sessionService.clearSession();
    void this.router.navigate(['/create-an-account']);
  }

  openChangePassword(): void {
    void this.router.navigate(
      ['/update-password'],
      { queryParams: { email: this.email.trim() } }
    );
  }

  openForgotPassword(): void {
    void this.router.navigate(['/forgot-password']);
  }

  onSubmit(form: NgForm): void {
    this.login(form);
  }
}
