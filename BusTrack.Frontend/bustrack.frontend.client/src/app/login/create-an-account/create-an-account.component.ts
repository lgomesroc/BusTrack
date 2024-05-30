import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { NgForm } from '@angular/forms'; // Importar NgForm
import { startInactivityTimerRule } from '../rules/inactivityTimerRules/inactivityTimerRule';
import { disableKeyboardShortcutsRule } from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import { blockSavePasswordRule } from '../rules/blockSavePasswordRules/blockSavePasswordRule'; // Importando a regra
import { ValidationService } from '../../services/validation.service'; // Importe o serviço

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
  INACTIVITY_TIMEOUT_MS = 1200000; // Adicionado
  cpfInvalido: boolean = false; // Adicione esta propriedade

  constructor(private router: Router, private http: HttpClient, private validationService: ValidationService) {
  }

  ngOnInit(): void {
    // Implementação de regras comuns a todas as telas
    disableKeyboardShortcutsRule();
    startInactivityTimerRule(this.INACTIVITY_TIMEOUT_MS);
    blockSavePasswordRule(); // Aplicando a regra de bloqueio de salvar senhas
    //this.bloquearSalvarSenhas();
  }

  /*bloquearSalvarSenhas(): void {
    const inputs = document.querySelectorAll('input[type="text"]');
    inputs.forEach(input => {
      input.setAttribute('autocomplete', 'new-password');
    });
  }*/

  login(form: NgForm): void {
    if (form.valid) {
      const credentials = { email: this.email, password: this.password };
      this.http.post<any>('http://localhost:7072/auth/login', credentials).subscribe({
        next: (response) => {
          if (response.success) {
            alert(response.welcomeMessage); // Usando 'response'
            alert('Sucesso. Seja bem-vindo ao painel principal do Bus Track');
            this.router.navigate(['/main-screen']);
          } else {
            alert('Login falhou. Por favor, verifique suas credenciais e tente novamente.');
          }
          // Login bem-sucedido
          alert('Sucesso. Seja bem-vindo ao painel principal do Bus Track');
        },
        error: err => {
          // Login falhou
          alert(`Erro ao fazer login: ${err.error.message}`); // Usando 'err'
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
      // Se o formulário não for válido, mostre uma mensagem de erro
      alert('Por favor, preencha todos os campos corretamente.');
    }
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
    if (!this.validationService.validarCPF(this.cpf)) {
      this.errorMessage = 'CPF inválido. Por favor, digite novamente.';
      return;
    }
    if (!this.validationService.validarSenha(this.password)) {
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
        if (response.contaCriadaComSucesso) {
          // Conta criada com sucesso, enviar o e-mail de confirmação
          this.enviarEmailConfirmacao(data.email);
        } else {
          // Se a resposta do servidor indicar que houve um problema
          // ao criar a conta, exibir a mensagem de erro.
          this.errorMessage = response.mensagemDeErro || 'Erro desconhecido ao criar conta.';
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
    // Remove caracteres não numéricos
    cpf = cpf.replace(/\D/g, '');

    // Verifica se o CPF tem 11 dígitos
    if (cpf.length !== 11) {
      this.cpfInvalido = true;
      return false;
    }

    // Verifica se todos os dígitos são iguais
    if (/^(\d)\1{10}$/.test(cpf)) {
      this.cpfInvalido = true;
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
      this.cpfInvalido = true;
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
      this.cpfInvalido = true;
      return false;
    }

    // CPF válido
    this.cpfInvalido = false;
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
    this.router.navigate(['/enter-the-system']);
  }
  onSubmit(form: NgForm) {
    if (form.valid) {
      this.login(form);
    } else {
      // Se o formulário não for válido, mostre uma mensagem de erro
      alert('Por favor, preencha todos os campos corretamente.');
    }
  }
}
