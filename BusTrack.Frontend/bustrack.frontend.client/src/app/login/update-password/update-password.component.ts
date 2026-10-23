import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { finalize } from 'rxjs';

import { environment } from '../../../environments/environment';
import { blockSavePasswordRule } from '../rules/blockSavePasswordRules/blockSavePasswordRule';
import { disableInteractionsRule } from '../rules/disableInteractionsRules/disableInteractionsRule';
import { disableKeyboardShortcutsRule } from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import { preventBackNavigationRule } from '../rules/preventBackNavigationRules/preventBackNavigationRule';
import { preventForwardNavigationRule } from '../rules/preventForwardNavigationRules/preventForwardNavigationRule';

interface ChangePasswordResponse {
  success: boolean;
}

@Component({
  selector: 'app-update-password',
  templateUrl: './update-password.component.html',
  styleUrls: ['./update-password.component.css']
})
export class UpdatePasswordComponent implements OnInit {
  email = '';
  currentPassword = '';
  newPassword = '';
  confirmPassword = '';

  errorMessage = '';
  successMessage = '';
  isSubmitting = false;

  private readonly genericErrorMessage =
    'Não foi possível alterar a senha. ' +
    'Verifique os dados informados e tente novamente.';

  constructor(
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly http: HttpClient
  ) {}

  ngOnInit(): void {
    blockSavePasswordRule();
    disableInteractionsRule();
    disableKeyboardShortcutsRule();
    preventBackNavigationRule();
    preventForwardNavigationRule();

    this.email =
      this.route.snapshot.queryParamMap.get('email')?.trim() ?? '';
  }

  updatePassword(form: NgForm): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.email) {
      this.errorMessage =
        'Não foi possível identificar a conta. Volte ao login e tente novamente.';
      return;
    }

    if (!form.valid) {
      this.errorMessage = 'Preencha todos os campos.';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage = 'As senhas não coincidem.';
      return;
    }

    if (!this.validatePassword(this.newPassword)) {
      this.errorMessage =
        'A senha deve ter no mínimo 8 caracteres, 2 letras minúsculas, ' +
        '2 maiúsculas, 2 números e 2 caracteres especiais.';
      return;
    }

    this.isSubmitting = true;

    this.http.post<ChangePasswordResponse>(
      `${environment.apiUrl}/AuthenticationControllerAPI/change-password`,
      {
        email: this.email,
        currentPassword: this.currentPassword,
        newPassword: this.newPassword,
        confirmPassword: this.confirmPassword
      }
    ).pipe(
      finalize(() => {
        this.isSubmitting = false;
      })
    ).subscribe({
      next: response => {
        if (!response?.success) {
          this.errorMessage = this.genericErrorMessage;
          return;
        }

        this.successMessage = 'Senha alterada com sucesso.';

        this.currentPassword = '';
        this.newPassword = '';
        this.confirmPassword = '';

        form.resetForm();

        window.setTimeout(() => {
          void this.router.navigate(['/enter-the-system']);
        }, 1200);
      },
      error: () => {
        this.errorMessage = this.genericErrorMessage;
      }
    });
  }

  validatePassword(password: string): boolean {
    return password.length >= 8
      && (password.match(/[a-z]/g) ?? []).length >= 2
      && (password.match(/[A-Z]/g) ?? []).length >= 2
      && (password.match(/[0-9]/g) ?? []).length >= 2
      && (password.match(/[^a-zA-Z0-9\s]/g) ?? []).length >= 2;
  }

  cancelar(): void {
    void this.router.navigate(['/enter-the-system']);
  }
}
