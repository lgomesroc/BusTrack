import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { startInactivityTimerRule } from '../rules/inactivityTimerRules/inactivityTimerRule';
import { disableKeyboardShortcutsRule } from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import { preventBackNavigationRule } from '../rules/preventBackNavigationRules/preventBackNavigationRule';
import { preventForwardNavigationRule } from '../rules/preventForwardNavigationRules/preventForwardNavigationRule';

@Component({
  selector: 'app-confirmation', 
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
  INACTIVITY_TIMEOUT_MS = 1200000; 

  constructor(private router: Router, private http: HttpClient) {
  }

  ngOnInit(): void {
    disableKeyboardShortcutsRule();
    startInactivityTimerRule(this.INACTIVITY_TIMEOUT_MS);
    preventBackNavigationRule();
    preventForwardNavigationRule();
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
        this.router.navigate(['/conclusion']);
      },
      error: err => {
        console.error('Erro ao enviar e-mail de confirmação:', err);
      }
    });
  }

  confirmarCadastro(): void {
    this.enviarEmailConfirmacao(this.email);
  }
}
