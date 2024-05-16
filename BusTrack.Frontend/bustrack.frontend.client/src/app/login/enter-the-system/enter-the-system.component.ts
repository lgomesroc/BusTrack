import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { startInactivityTimerRule } from '../rules/inactivityTimerRules/inactivityTimerRule';
import { disableKeyboardShortcutsRule } from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';

@Component({
  selector: 'app-login',
  templateUrl: './enter-the-system.component.html',
  styleUrls: ['./enter-the-system.component.css']
})
export class EnterTheSystemComponent implements OnInit {
  email: string = '';
  password: string = '';
  inactivityTimer: any;
  loginAttempts: number = 5;
  INACTIVITY_TIMEOUT_MS = 1200000; // Adicionado

  constructor(private router: Router, private http: HttpClient) {
  }

  ngOnInit(): void {
    // Implementação de regras comuns a todas as telas
    disableKeyboardShortcutsRule();
    startInactivityTimerRule(this.INACTIVITY_TIMEOUT_MS);
  }

  login(): void {
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
  }

  cancelar(): void {
    // Implemente a lógica para cancelar a operação
    // Por exemplo, você pode querer navegar de volta para a tela principal
    this.router.navigate(['/main-screen']);
  }

  voltar(): void {
    // Implemente a lógica para voltar para a tela anterior ou para a tela de criação de conta
    // Por exemplo, você pode querer navegar para a tela de criação de conta
    this.router.navigate(['/create-account']);
  }
}
