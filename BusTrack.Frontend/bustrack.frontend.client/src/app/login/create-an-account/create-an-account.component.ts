import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { NgForm } from '@angular/forms'; 
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

  constructor(private router: Router, private http: HttpClient, private validationService: ValidationService) {
  }

  ngOnInit(): void {
    disableKeyboardShortcutsRule();
    startInactivityTimerRule(this.INACTIVITY_TIMEOUT_MS);
    blockSavePasswordRule(); 
  }

    login(form: NgForm): void {
    if (form.valid) {
      const credentials = { email: this.email, password: this.password };
      this.http.post<any>('http://localhost:7072/auth/login', credentials).subscribe({
        next: (response) => {
          if (response.success) {
            alert(response.welcomeMessage); 
            alert('Sucesso. Seja bem-vindo ao painel principal do Bus Track');
            this.router.navigate(['/main-screen']);
          } else {
            alert('Login falhou. Por favor, verifique suas credenciais e tente novamente.');
          }
          alert('Sucesso. Seja bem-vindo ao painel principal do Bus Track');
        },
        error: err => {
          alert(`Erro ao fazer login: ${err.error.message}`); 
          if (this.loginAttempts > 0) {
            alert(`E-mail, senha ou ambos não encontrados. Por favor, tente novamente. Tentativas restantes: ${this.loginAttempts}`);
            this.loginAttempts--;
          } else {
            alert('Você excedeu o número máximo de tentativas de login. Redirecionando para a tela de criar conta.');
            this.router.navigate(['/create-an-account']);
          }
        }
      });
    } else {
      alert('Por favor, preencha todos os campos corretamente.');
    }
  }

  criarConta(): void {
    if (!this.fullName || !this.cpf || !this.email || !this.password || !this.confirmPassword) {
      this.errorMessage = 'Por favor, preencha todos os campos.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'As senhas não coincidem. Por favor, digite novamente.';
      return;
    }

    if (!this.validationService.validarCPF(this.cpf)) {
      this.errorMessage = 'CPF inválido. Por favor, digite novamente.';
      return;
    }
    if (!this.validationService.validarSenha(this.password)) {
      this.errorMessage = 'Senha inválida. Por favor, escolha uma senha mais forte.';
      return;
    }

    if (this.loginAttempts <= 0) {
      this.errorMessage = 'Você excedeu o número máximo de tentativas de criar uma conta. Tente novamente mais tarde.';
      return;
    }

    const data = { fullName: this.fullName, cpf: this.cpf, email: this.email, password: this.password };
    this.http.post<any>('http://localhost:7072/verificar-cpf', { cpf: this.cpf }).subscribe({
      next: response => {
        if (response.cpfExists) {
          // CPF já existe no sistema
          this.errorMessage = 'Este CPF já está cadastrado. Clique em Login para acessar o sistema.';
        } else {
          this.criarContaNoBackend(data);
        }
      },
      error: err => {
        this.errorMessage = err.error.message;
        this.loginAttempts--; 
      }
    });
  }

  criarContaNoBackend(data: any): void {
    this.http.post<any>('http://localhost:7072/criar-conta', data).subscribe({
      next: (response) => {
        if (response.contaCriadaComSucesso) {
          this.enviarEmailConfirmacao(data.email);
        } else {
          this.errorMessage = response.mensagemDeErro || 'Erro desconhecido ao criar conta.';
        }
      },
      error: (err) => {
        this.errorMessage = err.error.message;
        this.loginAttempts--; 
      }
    });
  }

  enviarEmailConfirmacao(email: string): void {
    const mensagem = `Prezado(a), <br><br>
      Verificamos que você se cadastrou no nosso sistema e solicitamos que clique no link abaixo ou no botão "Confirmar" para finalizar o cadastro.<br><br>
      <a href="http://localhost:7072/confirmar-cadastro/${email}">Confirmar</a><br><br>
      Você tem 30 minutos para confirmar. <br><br>
      Atenciosamente, <br>
      Equipe do Sistema Bus Track`;

    this.http.post<any>('http://localhost:7072/enviar-email-confirmacao', { email, mensagem }).subscribe({
      next: response => {
        console.log('E-mail de confirmação enviado:', response);
        this.router.navigate(['/confirmation']);
      },
      error: err => {
        console.error('Erro ao enviar e-mail de confirmação:', err);
      }
    });
  }

  registrarDadosLoginSenha(email: string, password: string): void {
    const data = { email, password };
    this.http.post<any>('http://localhost:7072/registrar-dados-login-senha', data).subscribe({
      next: response => {
        console.log('Dados de login e senha registrados com sucesso:', response);
      },
      error: err => {
        console.error('Erro ao registrar dados de login e senha:', err);
      }
    });
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
  onSubmit(form: NgForm) {
    if (form.valid) {
      this.login(form);
    } else {
      alert('Por favor, preencha todos os campos corretamente.');
    }
  }
}
