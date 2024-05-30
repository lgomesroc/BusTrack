import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { UserService } from '../../services/user.service';
import { UserData } from '../../models/user.model';
import { blockSavePasswordRule } from '../rules/blockSavePasswordRules/blockSavePasswordRule';
import { disableInteractionsRule } from '../rules/disableInteractionsRules/disableInteractionsRule';
import { disableKeyboardShortcutsRule } from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import { preventBackNavigationRule } from '../rules/preventBackNavigationRules/preventBackNavigationRule';
import { preventForwardNavigationRule } from '../rules/preventForwardNavigationRules/preventForwardNavigationRule';
import { startSessionTimeoutRule, clearSessionTimeoutRule } from '../rules/sessionTimeoutRules/sessionTimeoutRule';
import { startInactivityTimerRule, clearInactivityTimerRule } from '../rules/inactivityTimerRules/inactivityTimerRule';

@Component({
  selector: 'app-update-password',
  templateUrl: './update-password.component.html',
  styleUrls: ['./update-password.component.css']
})
export class UpdatePasswordComponent implements OnInit {
  fullName: string = '';
  cpf: string = '';
  email: string = '';
  newPassword: string = '';
  confirmPassword: string = '';
  errorMessage: string = '';

  constructor(private router: Router, private http: HttpClient, private userService: UserService) { }

  ngOnInit(): void {
    blockSavePasswordRule();
    disableInteractionsRule();
    disableKeyboardShortcutsRule();
    preventBackNavigationRule();
    preventForwardNavigationRule();
    startSessionTimeoutRule(180000);
    clearSessionTimeoutRule();
    startInactivityTimerRule(120000);
    clearInactivityTimerRule();
    // Recupere os dados do usuário
    this.userService.getUserData().subscribe((userData: UserData) => {
      this.fullName = userData.fullName;
      this.cpf = userData.cpf;
      this.email = userData.email;
    });
  }

  updatePassword(form: NgForm): void {
    if (form.valid) {
      if (this.newPassword !== this.confirmPassword) {
        this.errorMessage = 'As senhas não coincidem. Por favor, digite novamente.';
        return;
      }

      if (!this.validatePassword(this.newPassword)) {
        this.errorMessage = 'Senha inválida. Por favor, escolha uma senha mais forte.';
        return;
      }

      const data = { email: this.email, newPassword: this.newPassword };
      this.http.post<any>('http://localhost:7072/auth/login', data).subscribe({
        next: (response) => {
          if (response.senhaAtualizadaComSucesso) {
            alert('Senha atualizada com sucesso!');
            this.router.navigate(['/enter-the-system']);
          } else {
            this.errorMessage = response.mensagemDeErro || 'Erro desconhecido ao atualizar a senha.';
          }
        },
        error: (err) => {
          this.errorMessage = `Ocorreu um erro ao atualizar a senha: ${err.error.message}`;
        }
      });
    } else {
      alert('Por favor, preencha todos os campos corretamente.');
    }
  }

  validatePassword(password: string): boolean {
    if (password.length < 8) {
      return false;
    }

    if ((password.match(/[A-Z]/g) || []).length < 2) {
      return false;
    }

    if ((password.match(/[a-z]/g) || []).length < 2) {
      return false;
    }

    if (!/(?!.*(\d)\1)(?!.*(\d)(\d)\2)\d{2}/.test(password)) {
      return false;
    }

    if (!/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/.test(password)) {
      return false;
    }

    return true;
  }

  cancelar(): void {
    this.router.navigate(['/main-screen']);
  }
}
