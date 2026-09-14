import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { startInactivityTimerRule } from '../rules/inactivityTimerRules/inactivityTimerRule';
import { disableKeyboardShortcutsRule } from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import { blockSavePasswordRule } from '../rules/blockSavePasswordRules/blockSavePasswordRule';
import { ValidationService } from '../../services/validation.service';

@Component({
  selector: 'app-create-an-account',
  templateUrl: './create-an-account.component.html',
  styleUrls: ['./create-an-account.component.css']
})
export class CreateAnAccountComponent implements OnInit {
  fullName: string = '';
  cpf: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  errorMessage: string = '';
  loginAttempts: number = 5;
  inactivityTimer: any;
  INACTIVITY_TIMEOUT_MS = 1200000;
  cpfInvalido: boolean = false;

  private readonly apiUrl = 'http://localhost:5066';

  constructor(
    private router: Router,
    private http: HttpClient,
    private validationService: ValidationService
  ) {
  }

  ngOnInit(): void {
    disableKeyboardShortcutsRule();
    startInactivityTimerRule(this.INACTIVITY_TIMEOUT_MS);
    blockSavePasswordRule();
  }

  criarConta(): void {
    this.errorMessage = '';

    if (!this.fullName || !this.cpf || !this.email || !this.password || !this.confirmPassword) {
      this.errorMessage = 'Por favor, preencha todos os campos.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'As senhas não coincidem. Por favor, digite novamente.';
      return;
    }

    const cpfSemMascara = this.cpf.replace(/\D/g, '');

    if (!this.validationService.validarCPF(cpfSemMascara)) {
      this.errorMessage = 'CPF inválido. Por favor, digite novamente.';
      this.cpfInvalido = true;
      return;
    }

    this.cpfInvalido = false;

    if (!this.validationService.validarSenha(this.password)) {
      this.errorMessage = 'Senha inválida. Por favor, escolha uma senha mais forte.';
      return;
    }

    if (this.loginAttempts <= 0) {
      this.errorMessage =
        'Você excedeu o número máximo de tentativas de criar uma conta. Tente novamente mais tarde.';
      return;
    }

    const data = {
      fullName: this.fullName,
      cpf: cpfSemMascara,
      email: this.email,
      password: this.password
    };

    this.http.post<any>(`${this.apiUrl}/AccountControllerAPI`, data).subscribe({
      next: (response) => {
        if (response?.success) {
          alert(response.message || 'Conta criada com sucesso.');
          this.router.navigate(['/confirmation']);
        } else {
          this.errorMessage =
            response?.message || 'Não foi possível criar a conta.';
        }
      },
      error: (err) => {
        console.error('Erro ao criar conta:', err);

        if (err.status === 409) {
          this.errorMessage =
            err.error?.message || 'Já existe uma conta com esses dados.';
        } else if (err.status === 400) {
          this.errorMessage =
            err.error?.message || 'Os dados informados são inválidos.';
        } else if (err.status === 0) {
          this.errorMessage =
            'Não foi possível conectar ao backend. Verifique se a API está em execução.';
        } else {
          this.errorMessage =
            err.error?.message || 'Erro ao criar a conta.';
        }

        this.loginAttempts--;
      }
    });
  }

  formatarCPF(value: string): void {
    let cpf = value.replace(/\D/g, '');

    if (cpf.length > 11) {
      cpf = cpf.substring(0, 11);
    }

    if (cpf.length <= 3) {
      this.cpf = cpf;
      return;
    }

    if (cpf.length <= 6) {
      this.cpf = `${cpf.substring(0, 3)}.${cpf.substring(3)}`;
      return;
    }

    if (cpf.length <= 9) {
      this.cpf =
        `${cpf.substring(0, 3)}.${cpf.substring(3, 6)}.${cpf.substring(6)}`;
      return;
    }

    this.cpf =
      `${cpf.substring(0, 3)}.${cpf.substring(3, 6)}.${cpf.substring(6, 9)}-${cpf.substring(9, 11)}`;
  }

  validarCPF(cpf: string): boolean {
    cpf = cpf.replace(/\D/g, '');

    if (cpf.length !== 11) {
      this.cpfInvalido = true;
      return false;
    }

    if (/^(\d)\1{10}$/.test(cpf)) {
      this.cpfInvalido = true;
      return false;
    }

    let sum = 0;

    for (let i = 0; i < 9; i++) {
      sum += parseInt(cpf.charAt(i)) * (10 - i);
    }

    let remainder = sum % 11;
    let digit = remainder < 2 ? 0 : 11 - remainder;

    if (digit !== parseInt(cpf.charAt(9))) {
      this.cpfInvalido = true;
      return false;
    }

    sum = 0;

    for (let i = 0; i < 10; i++) {
      sum += parseInt(cpf.charAt(i)) * (11 - i);
    }

    remainder = sum % 11;
    digit = remainder < 2 ? 0 : 11 - remainder;

    if (digit !== parseInt(cpf.charAt(10))) {
      this.cpfInvalido = true;
      return false;
    }

    this.cpfInvalido = false;
    return true;
  }

  validarSenha(password: string): boolean {
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

  realizarLogin(): void {
    this.router.navigate(['/enter-the-system']);
  }
}
