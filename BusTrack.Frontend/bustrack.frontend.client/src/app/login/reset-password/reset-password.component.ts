import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';

import { environment } from '../../../environments/environment';

interface ResetPasswordResponse {
  success?: boolean;
}

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  email = '';
  recoveryCode = '';
  newPassword = '';
  confirmPassword = '';

  errorMessage = '';
  successMessage = '';
  isSubmitting = false;

  private readonly genericErrorMessage =
    'Não foi possível validar a recuperação. ' +
    'Verifique os dados informados e tente novamente.';

  constructor(
    private readonly http: HttpClient,
    private readonly route: ActivatedRoute,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.email = (
      this.route.snapshot.queryParamMap.get('email') ?? ''
    ).trim();
  }

  submit(): void {
    const email = this.email.trim();
    const recoveryCode = this.recoveryCode.trim();

    this.errorMessage = '';
    this.successMessage = '';

    if (
      !email ||
      !recoveryCode ||
      !this.newPassword ||
      !this.confirmPassword
    ) {
      this.errorMessage = 'Preencha todos os campos.';
      return;
    }

    if (this.newPassword.length < 8) {
      this.errorMessage =
        'A senha deve ter pelo menos 8 caracteres.';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage = 'As senhas não coincidem.';
      return;
    }

    this.isSubmitting = true;

    this.http.post<ResetPasswordResponse>(
      `${environment.apiUrl}/AuthenticationControllerAPI/reset-password`,
      {
        email,
        recoveryCode,
        newPassword: this.newPassword
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

        this.successMessage =
          'Senha redefinida com sucesso.';

        this.recoveryCode = '';
        this.newPassword = '';
        this.confirmPassword = '';
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
