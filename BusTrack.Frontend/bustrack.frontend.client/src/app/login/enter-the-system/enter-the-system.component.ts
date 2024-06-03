import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { NgForm } from '@angular/forms'; // Importar NgForm
import { startInactivityTimerRule } from '../rules/inactivityTimerRules/inactivityTimerRule';
import { disableKeyboardShortcutsRule } from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import { preventBackNavigationRule } from '../rules/preventBackNavigationRules/preventBackNavigationRule';
import { preventForwardNavigationRule } from '../rules/preventForwardNavigationRules/preventForwardNavigationRule';
import { blockSavePasswordRule } from '../rules/blockSavePasswordRules/blockSavePasswordRule'; 


@Component({
  selector: 'app-enter-the-system',
  templateUrl: './enter-the-system.component.html',
  styleUrls: ['./enter-the-system.component.css']
})
export class EnterTheSystemComponent implements OnInit {
  email: string = '';
  password: string = '';
  errorMessage: string = '';
  loginAttempts: number = 5;
  inactivityTimer: any;
  INACTIVITY_TIMEOUT_MS = 1200000; 
  totalLoginAttempts: number = 5; 

  constructor(private router: Router, private http: HttpClient) {
  }

  ngOnInit(): void {
    disableKeyboardShortcutsRule();
    startInactivityTimerRule(this.INACTIVITY_TIMEOUT_MS);
    preventBackNavigationRule();
    preventForwardNavigationRule();
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
            this.router.navigate(['/dashboard']);
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


  cancelar(): void {
    this.email = '';
    this.password = '';
    alert('Operação de login cancelada.');
    this.router.navigate(['/main-screen']);
  }

  voltar(): void {
    const isFirstLoginAttempt = this.loginAttempts === this.totalLoginAttempts;
    if (isFirstLoginAttempt) {
      this.router.navigate(['/create-an-account']);
    } else {
      this.router.navigate(['/create-an-account']);
    }
  }

  onSubmit(form: NgForm) {
    if (form.valid) {
      this.login(form);
    } else {
      alert('Por favor, preencha todos os campos corretamente.');
    }
  }
}
