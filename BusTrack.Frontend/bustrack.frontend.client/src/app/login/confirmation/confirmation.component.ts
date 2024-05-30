import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { startInactivityTimerRule } from '../rules/inactivityTimerRules/inactivityTimerRule';
import { disableKeyboardShortcutsRule } from '../rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import { preventBackNavigationRule } from '../rules/preventBackNavigationRules/preventBackNavigationRule';
import { preventForwardNavigationRule } from '../rules/preventForwardNavigationRules/preventForwardNavigationRule';

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
    preventBackNavigationRule();
    preventForwardNavigationRule();
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
        // Redirecionar para a tela de conclusão
        this.router.navigate(['/conclusion']);
      },
      error: err => {
        // Lidar com erros de envio de e-mail
        console.error('Erro ao enviar e-mail de confirmação:', err);
      }
    });
  }

  confirmarCadastro(): void {
    // Aqui você deve implementar a lógica para confirmar o cadastro
    // Por exemplo, você pode chamar um serviço que envia uma requisição para o seu backend
    this.enviarEmailConfirmacao(this.email);
  }
}
