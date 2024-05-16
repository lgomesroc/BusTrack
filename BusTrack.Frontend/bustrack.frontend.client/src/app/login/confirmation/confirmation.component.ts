import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { startInactivityTimerRule } from '../rules/inactivityTimerRules/inactivityTimerRule';
import { disableKeyboardShortcutsRule } from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';

@Component({
  selector: 'app-confirmation', // Seletor corrigido para 'app-confirmation'
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.css']
})
export class ConfirmationComponent implements OnInit {
  fullName: string = '';
  cpf: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  errorMessage: string = '';
  loginAttempts: number = 5;
  inactivityTimer: any;
  INACTIVITY_TIMEOUT_MS = 1200000; // Adicionado

  constructor(private router: Router, private http: HttpClient) {
  }

  ngOnInit(): void {
    // Implementação de regras comuns a todas as telas
    disableKeyboardShortcutsRule();
    startInactivityTimerRule(this.INACTIVITY_TIMEOUT_MS);
    this.bloquearSalvarSenhas();
  }

  bloquearSalvarSenhas(): void {
    const inputs = document.querySelectorAll('input[type="text"]');
    inputs.forEach(input => {
      input.setAttribute('autocomplete', 'new-password');
    });
  }

  criarConta(): void {
    // Verifica se os campos foram preenchidos corretamente
    if (!this.fullName || !this.cpf || !this.email || !this.password || !this.confirmPassword) {
      this.errorMessage = 'Por favor, preencha todos os campos.';
      return;
    }

    // Verifica se as senhas coincidem
    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'As senhas não coincidem. Por favor, digite novamente.';
      return;
    }

    // Lógica para verificar se o CPF é válido
    if (!this.validarCPF(this.cpf)) {
      this.errorMessage = 'CPF inválido. Por favor, digite novamente.';
      return;
    }

    // Lógica para verificar se a senha atende aos requisitos
    if (!this.validarSenha(this.password)) {
      this.errorMessage = 'Senha inválida. Por favor, escolha uma senha mais forte.';
      return;
    }

    // Verifica se o usuário esgotou as tentativas de login
    if (this.loginAttempts <= 0) {
      this.errorMessage = 'Você excedeu o número máximo de tentativas de criar uma conta. Tente novamente mais tarde.';
      return;
    }

    // Enviar os dados para o backend para verificar se o CPF já existe no banco de dados
    const data = { fullName: this.fullName, cpf: this.cpf, email: this.email, password: this.password };
    this.http.post<any>('http://localhost:7072/verificar-cpf', { cpf: this.cpf }).subscribe({
      next: response => {
        if (response.cpfExists) {
          // CPF já existe no sistema
          this.errorMessage = 'Este CPF já está cadastrado. Clique em Login para acessar o sistema.';
        } else {
          // CPF não existe, continuar o cadastro
          // Chamar o método para criar a conta no backend
          this.criarContaNoBackend(data);
        }
      },
      error: err => {
        // O backend retornou um erro, exibir mensagem de erro adequada
        this.errorMessage = err.error.message;
        this.loginAttempts--; // Decrementa as tentativas restantes
      }
    });
  }

  criarContaNoBackend(data: any): void {
    this.http.post<any>('http://localhost:7072/criar-conta', data).subscribe({
      next: (response) => {
        // Verifica se a conta foi criada com sucesso
        if (response.success) {
          // Conta criada com sucesso, enviar o e-mail de confirmação
          this.enviarEmailConfirmacao(data.email);
        } else {
          // Se a resposta do servidor indicar que houve um problema
          // ao criar a conta, exibir a mensagem de erro.
          this.errorMessage = response.message;
        }
      },
      error: (err) => {
        // O backend retornou um erro, exibir mensagem de erro adequada
        this.errorMessage = err.error.message;
        this.loginAttempts--; // Decrementa as tentativas restantes
      }
    });
  }

  enviarEmailConfirmacao(email: string): void {
    // Construir a mensagem de e-mail de confirmação
    const mensagem = `Prezado(a), <br><br>
      Verificamos que você se cadastrou no nosso sistema e solicitamos que clique no link abaixo ou no botão "Confirmar" para finalizar o cadastro.<br><br>
      <a href="http://localhost:7072/confirmar-cadastro/${email}">Confirmar</a><br><br>
      Você tem 30 minutos para confirmar. <br><br>
      Atenciosamente, <br>
      Equipe do Sistema Bus Track`;

    // Enviar o e-mail de confirmação para o endereço fornecido
    this.http.post<any>('http://localhost:7072/enviar-email-confirmacao', { email, mensagem }).subscribe({
      next: response => {
        // E-mail de confirmação enviado com sucesso
        console.log('E-mail de confirmação enviado:', response);
        // Redirecionar para a tela de confirmação
        this.router.navigate(['/confirmation']);
      },
      error: err => {
        // Lidar com erros de envio de e-mail
        console.error('Erro ao enviar e-mail de confirmação:', err);
      }
    });
  }

  validarCPF(cpf: string): boolean {
    // Remove caracteres não numéricos
    cpf = cpf.replace(/\D/g, '');

    // Verifica se o CPF tem 11 dígitos
    if (cpf.length !== 11) {
      return false;
    }

    // Verifica se todos os dígitos são iguais
    if (/^(\d)\1{10}$/.test(cpf)) {
      return false;
    }

    // Calcula o primeiro dígito verificador
    let sum = 0;
    for (let i = 0; i < 9; i++) {
      sum += parseInt(cpf.charAt(i)) * (10 - i);
    }
    let remainder = sum % 11;
    let digit = remainder < 2 ? 0 : 11 - remainder;

    // Verifica se o primeiro dígito verificador é igual ao do CPF
    if (digit !== parseInt(cpf.charAt(9))) {
      return false;
    }

    // Calcula o segundo dígito verificador
    sum = 0;
    for (let i = 0; i < 10; i++) {
      sum += parseInt(cpf.charAt(i)) * (11 - i);
    }
    remainder = sum % 11;
    digit = remainder < 2 ? 0 : 11 - remainder;

    // Verifica se o segundo dígito verificador é igual ao do CPF
    if (digit !== parseInt(cpf.charAt(10))) {
      return false;
    }

    // CPF válido
    return true;
  }

  validarSenha(password: string): boolean {
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

  cancelar(): void {
    // Redirecionar para a tela principal
    this.router.navigate(['/main-screen']);
  }

  realizarLogin(): void {
    // Redirecionar para a tela de login
    this.router.navigate(['/login']);
  }
  confirmarCadastro(): void {
    // Aqui você deve implementar a lógica para confirmar o cadastro
    // Por exemplo, você pode chamar um serviço que envia uma requisição para o seu backend
    this.enviarEmailConfirmacao(this.email);
  }
}
