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

import {
  SessionService
} from '../../services/session.service';

@Component({
  selector: 'app-enter-the-system',
  templateUrl: './enter-the-system.component.html',
  styleUrls: ['./enter-the-system.component.css']
})
export class EnterTheSystemComponent implements OnInit {

  email: string = '';

  password: string = '';

  errorMessage: string = '';

  loginAttempts: number = 5;

  totalLoginAttempts: number = 5;

  constructor(
    private router: Router,
    private http: HttpClient,
    private sessionService: SessionService
  ) {
  }

  ngOnInit(): void {

    disableKeyboardShortcutsRule();

    preventBackNavigationRule();

    preventForwardNavigationRule();

    blockSavePasswordRule();
  }

  login(form: NgForm): void {

    if (!form.valid) {

      alert(
        'Por favor, preencha todos os campos corretamente.'
      );

      return;
    }

    const credentials = {
      email: this.email,
      password: this.password
    };

    this.http.post<any>(
      'http://localhost:5066/AuthenticationControllerAPI/login',
      credentials
    ).subscribe({

      next: (response) => {

        if (response.success) {

          /*
           * Marca a sessão como autenticada.
           *
           * O AuthenticationGuard utiliza essa informação
           * para permitir o acesso ao Dashboard e às demais
           * telas protegidas.
           */
          this.sessionService.startSession();

          alert(
            response.welcomeMessage ??
            'Sucesso. Seja bem-vindo ao painel principal do Bus Track.'
          );

          this.router.navigate(
            ['/dashboard']
          );

          return;
        }

        alert(
          'Login falhou. Por favor, verifique suas credenciais e tente novamente.'
        );
      },

      error: (err) => {

        if (this.loginAttempts > 0) {

          const message =
            err?.error?.message ??
            'E-mail, senha ou ambos não encontrados. Por favor, tente novamente.';

          alert(
            `${message}\n\nTentativas restantes: ${this.loginAttempts}`
          );

          this.loginAttempts--;

          return;
        }

        alert(
          'Você excedeu o número máximo de tentativas de login. Redirecionando para a tela de criar conta.'
        );

        this.sessionService.clearSession();

        this.router.navigate(
          ['/create-an-account']
        );
      }
    });
  }

  cancelar(): void {

    this.email = '';

    this.password = '';

    this.sessionService.clearSession();

    alert(
      'Operação de login cancelada.'
    );

    this.router.navigate(
      ['/main-screen']
    );
  }

  voltar(): void {

    this.sessionService.clearSession();

    this.router.navigate(
      ['/create-an-account']
    );
  }

  onSubmit(form: NgForm): void {

    this.login(form);
  }
}
