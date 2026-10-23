
import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';

interface RecoveryResponse {
  success?: boolean;
  message?: string;
}

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  email = '';
  newPassword = '';
  confirmPassword = '';

  errorMessage = '';
  successMessage = '';
  isSubmitting = false;

  private readonly genericErrorMessage =
    'Não foi possível concluir a recuperação. Verifique os dados informados.';

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router
  ) {}

  submit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    const email = this.email.trim();

    if (
      !email ||
      !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email) ||
      !this.newPassword ||
      !this.confirmPassword
    ) {
      this.errorMessage = 'Preencha os três campos corretamente.';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage = 'As senhas não coincidem.';
      return;
    }

    if (this.newPassword.length < 8) {
      this.errorMessage = 'A senha deve ter pelo menos 8 caracteres.';
      return;
    }

    this.isSubmitting = true;

    this.http.post<RecoveryResponse>(
      `${environment.apiUrl}/AuthenticationControllerAPI/forgot-password`,
      {
        email,
        newPassword: this.newPassword,
        confirmPassword: this.confirmPassword
      }
    ).pipe(
      finalize(() => {
        this.isSubmitting = false;
      })
    ).subscribe({
      next: response => {
        if (response?.success === false) {
          this.errorMessage = this.genericErrorMessage;
          return;
        }

        this.successMessage =
          response?.message ?? 'Senha alterada com sucesso. Entre novamente.';

        this.newPassword = '';
        this.confirmPassword = '';

        setTimeout(() => {
          void this.router.navigate(['/enter-the-system']);
        }, 900);
      },
      error: () => {
        this.errorMessage = this.genericErrorMessage;
      }
    });
  }

  backToLogin(): void {
    void this.router.navigate(['/enter-the-system']);
  }
}
