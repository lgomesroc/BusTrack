import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  currentForm: 'login' | 'create' = 'login'; // Define o formulário atual como login por padrão
  loginData = { email: '', password: '' }; // Dados do formulário de login
  loginError = ''; // Mensagem de erro de login
  loginAttempts = 5; // Número máximo de tentativas de login permitidas
  createData = { fullName: '', cpf: '', email: '', password: '' }; // Dados do formulário de criação de conta
  createForm: FormGroup; // Formulário de criação de conta
  passwordStrength: 'weak' | 'strong' = 'weak'; // Força da senha
  cpfError = ''; // Mensagem de erro do CPF

  constructor(private formBuilder: FormBuilder, private http: HttpClient) {
    this.createForm = this.formBuilder.group({
      fullName: ['', Validators.required],
      cpf: ['', [Validators.required, Validators.pattern('[0-9]{3}\\.[0-9]{3}\\.[0-9]{3}-[0-9]{2}')]],
      email: ['', Validators.required],
      password: ['', Validators.required]
    });
  }


  toggleForm(formName: 'login' | 'create') {
    this.currentForm = formName;
    this.loginError = ''; // Limpa a mensagem de erro ao trocar de formulário
  }

  login() {
    // Lógica de autenticação simulada
    const { email, password } = this.loginData;

    // Simulação de verificação de credenciais
    if (email === 'usuario@exemplo.com' && password === 'senha123') {
      // Login bem-sucedido, redirecionar para a página principal ou executar outra ação
      console.log('Login bem-sucedido');
    } else {
      // Decrementa o número de tentativas restantes
      this.loginAttempts--;

      if (this.loginAttempts > 0) {
        // Mensagem de erro indicando tentativas restantes
        this.loginError = `Login incorreto, senha incorreta ou ambas. Você tem mais ${this.loginAttempts} tentativa(s) para corrigir.`;
      } else {
        // Bloqueia o login após exceder o número máximo de tentativas
        this.loginError = 'Você excedeu o número máximo de tentativas de login. Por favor, tente novamente mais tarde.';
      }
    }
  }

  checkPasswordStrength(password: string) {
    // Verifica a força da senha
    const regexUpperCase = /[A-Z]/;
    const regexLowerCase = /[a-z]/;
    const regexNumbers = /[0-9]/;
    const regexSpecialChars = /[!@#$%^&*(),.?":{}|<>]/;

    const hasUpperCase = regexUpperCase.test(password);
    const hasLowerCase = regexLowerCase.test(password);
    const hasNumbers = regexNumbers.test(password);
    const hasSpecialChars = regexSpecialChars.test(password);

    if (password.length >= 8 && hasUpperCase && hasLowerCase && hasNumbers && hasSpecialChars) {
      // Verifica se não há números repetidos ou sequenciais
      if (!this.hasRepeatedOrSequentialNumbers(password)) {
        this.passwordStrength = 'strong';
      } else {
        this.passwordStrength = 'weak';
      }
    } else {
      this.passwordStrength = 'weak';
    }
  }

  hasRepeatedOrSequentialNumbers(password: string): boolean {
    // Verifica se há números repetidos ou sequenciais
    for (let i = 0; i < password.length - 1; i++) {
      const currentDigit = parseInt(password[i], 10);
      const nextDigit = parseInt(password[i + 1], 10);

      if (currentDigit + 1 === nextDigit || currentDigit - 1 === nextDigit || currentDigit === nextDigit) {
        return true;
      }
    }
    return false;
  }

  createAccount() {
    // Verificar se o email já está em uso
    this.http.get<any>('localhost:3000/verificar-email/' + this.createForm.value.email).subscribe(response => {
      if (response.emailExists) {
        // Se o email já existe, exibir uma mensagem de erro
        console.log('Erro: O email já está em uso.');
      } else {
        // Se o email não existe, salvar os dados no banco de dados
        this.http.post<any>('URL_DO_BACKEND/salvar-usuario', this.createForm.value).subscribe(saveResponse => {
          console.log('Usuário criado com sucesso:', saveResponse);
          // Você pode adicionar redirecionamento ou outras ações aqui, se necessário
        });
      }
    }, error => {
      console.error('Erro ao verificar o email:', error);
    });
  }

  formatCPF() {
    // Formata o CPF como 000.000.000-00
    let cpf = this.createData.cpf.replace(/\D/g, '');
    cpf = cpf.substring(0, 11);
    cpf = cpf.replace(/(\d{3})(\d)/, '$1.$2');
    cpf = cpf.replace(/(\d{3})(\d)/, '$1.$2');
    cpf = cpf.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    this.createData.cpf = cpf;
  }
} 
