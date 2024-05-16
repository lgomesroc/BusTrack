// update-password.component.ts

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { blockSavePasswordRule } from '../rules/blockSavePasswordRules/blockSavePasswordRule'; // Importando a regra
import { disableInteractionsRule } from '../rules/disableInteractionsRules/disableInteractionsRule'; // Importando a regra
import { disableKeyboardShortcutsRule } from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule'; // Importando a regra
import { preventBackNavigationRule } from '../rules/preventBackNavigationRules/preventBackNavigationRule'; // Importando a regra
import { preventForwardNavigationRule } from '../rules/preventForwardNavigationRules/preventForwardNavigationRule'; // Importando a regra
import { startSessionTimeoutRule, clearSessionTimeoutRule } from '../rules/sessionTimeoutRules/sessionTimeoutRule'; // Importando a regra
import { startInactivityTimerRule, clearInactivityTimerRule } from '../rules/inactivityTimerRules/inactivityTimerRule'; // Importando a regra

@Component({
  selector: 'app-update-password',
  templateUrl: './update-password.component.html',
  styleUrls: ['./update-password.component.css']
})
export class UpdatePasswordComponent implements OnInit {
  fullName: string = ''; // Nome completo do usuário (visualização)
  cpf: string = ''; // CPF do usuário (visualização)
  email: string = ''; // E-mail do usuário (visualização)
  newPassword: string = ''; // Nova senha
  confirmPassword: string = ''; // Confirmar nova senha
  errorMessage: string = ''; // Mensagem de erro, se houver

  constructor(private router: Router, private http: HttpClient) { }

  ngOnInit(): void {
    // Implementação de regras comuns a todas as telas
    blockSavePasswordRule(); // Aplicando a regra de bloqueio de salvar senhas
    disableInteractionsRule(); // Aplicando a regra de desabilitar interações
    disableKeyboardShortcutsRule(); // Aplicando a regra de desabilitar atalhos de teclado
    preventBackNavigationRule(); // Aplicando a regra de prevenir navegação para trás
    preventForwardNavigationRule(); // Aplicando a regra de prevenir navegação para frente
    startSessionTimeoutRule(180000);
    clearSessionTimeoutRule(); // Aplicando a regra de tempo limite da sessão
    startInactivityTimerRule(120000); // Aplicando a regra de iniciar temporizador de inatividade
    clearInactivityTimerRule();
    // Aqui pode ser necessário preencher os campos de nome completo, CPF e e-mail com os dados do usuário
    this.fullName = 'Nome Completo do Usuário';
    this.cpf = '123.456.789-00';
    this.email = 'usuario@example.com';
  }

  updatePassword(): void {
    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage = 'As senhas não coincidem. Por favor, digite novamente.';
      return;
    }

    if (!this.validatePassword(this.newPassword)) {
      this.errorMessage = 'Senha inválida. Por favor, escolha uma senha mais forte.';
      return;
    }

    const data = { email: this.email, newPassword: this.newPassword };
    this.http.post<any>('URL_DO_BACKEND', data).subscribe({
      next: (response) => {
        // Verifica se a atualização da senha foi bem-sucedida
        if (response.senhaAtualizadaComSucesso) {
          alert('Senha atualizada com sucesso!');
          this.router.navigate(['/main-screen']);
        } else {
          // Se a resposta do servidor indicar que houve um problema
          // ao atualizar a senha, exibir a mensagem de erro.
          this.errorMessage = response.mensagemDeErro || 'Erro desconhecido ao atualizar a senha.';
        }
      },
      error: (err) => {
        // Exibe uma mensagem de erro com base na resposta do servidor
        this.errorMessage = `Ocorreu um erro ao atualizar a senha: ${err.error.message}`;
      }
    });
  }

  // Função para validar a senha
  validatePassword(password: string): boolean {
    // Verifica se a senha tem pelo menos 8 caracteres
    if (password.length < 8) {
      return false;
    }

    // Verifica se há pelo menos duas letras maiúsculas
    if ((password.match(/[A-Z]/g) || []).length < 2) {
      return false;
    }

    // Verifica se há pelo menos duas letras minúsculas
    if ((password.match(/[a-z]/g) || []).length < 2) {
      return false;
    }

    // Verifica se há pelo menos dois números que não sejam sequenciais nem repetidos
    if (!/(?!.*(\d)\1)(?!.*(\d)(\d)\2)\d{2}/.test(password)) {
      return false;
    }

    // Verifica se há pelo menos dois caracteres especiais
    if (!/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/.test(password)) {
      return false;
    }

    // Senha válida
    return true;
  }
}
