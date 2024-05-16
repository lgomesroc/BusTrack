import { Component, OnInit } from '@angular/core';
import { DataService } from '../../services/data.service';
import { blockSavePasswordRule } from '../../login/rules/blockSavePasswordRules/blockSavePasswordRule';
import { disableInteractionsRule, disableRightClick } from '../../login/rules/disableInteractionsRules/disableInteractionsRule';
import { disableKeyboardShortcutsRule } from '../../login/rules/disableKeyboardShortcutsRules/disableKeyboardShortcutsRule';
import { preventBackNavigationRule } from '../../login/rules/preventBackNavigationRules/preventBackNavigationRule';
import { startSessionTimeoutRule, clearSessionTimeoutRule } from '../../login/rules/sessionTimeoutRules/sessionTimeoutRule';
import { startInactivityTimerRule, clearInactivityTimerRule } from '../../login/rules/inactivityTimerRules/inactivityTimerRule';
import { validateFieldsRequiredRule } from '../rules-main/validateFieldsRequiredRules/validateFieldsRequiredRule';
import { validateEmailFormatRule } from '../rules-main/validateEmailFormatRules/validateEmailFormatRule';
import { limitCharactersRule } from '../rules-main/limitCharactersRules/limitCharactersRule';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  user: any; // Adicione esta linha
  constructor(private dataService: DataService) { } // Injetando o serviço de dados

  ngOnInit(): void {
    // Ativar regras de segurança
    blockSavePasswordRule();
    disableInteractionsRule();
    disableRightClick();
    disableKeyboardShortcutsRule();
    preventBackNavigationRule();

    // Aqui você precisa passar os argumentos corretos para as funções
    const emailInput = document.getElementById('email') as HTMLInputElement;
    validateEmailFormatRule(emailInput.value);

    const requiredField = document.getElementById('requiredField') as HTMLInputElement;
    validateFieldsRequiredRule(requiredField); // Passando 'requiredField' como argumento

    const characterLimitField = document.getElementById('characterLimitField') as HTMLInputElement;
    limitCharactersRule(characterLimitField, 200);

    // Iniciar temporizador de inatividade e timeout da sessão
    const inactivityTimeout = 600000; // 10 minutos de inatividade
    startInactivityTimerRule(inactivityTimeout);

    const sessionTimeout = 1800000; // 30 minutos de sessão
    startSessionTimeoutRule(sessionTimeout);

    // Limpar temporizadores ao sair do componente
    window.onunload = () => {
      clearInactivityTimerRule();
      clearSessionTimeoutRule();
    };

    this.user = {
      name: 'Nome do Usuário', // Substitua isso pelos dados reais do usuário
      id: 'ID do Usuário' // Substitua isso pelo ID real do usuário
      // outras propriedades do usuário aqui
    };
  }


  saveData() {
    // Aqui você pode coletar os dados do formulário e enviá-los para o seu serviço de dados
    const formData = {
      lineNumber: (document.getElementById('lineNumber') as HTMLInputElement).value,
      origin: (document.getElementById('origin') as HTMLInputElement).value,
      destination: (document.getElementById('destination') as HTMLInputElement).value,
      departureTime: (document.getElementById('departureTime') as HTMLInputElement).value,
      passengerAverage: (document.getElementById('passengerAverage') as HTMLInputElement).value
    };

    this.dataService.saveDataToMongoDB(formData).subscribe(response => {
      console.log('Dados salvos com sucesso:', response);
      // Aqui você pode adicionar qualquer lógica adicional após salvar os dados, como limpar o formulário ou exibir uma mensagem de sucesso
    }, error => {
      console.error('Erro ao salvar os dados:', error);
      // Aqui você pode adicionar lógica para lidar com erros de salvamento, como exibir uma mensagem de erro ao usuário
    });
  }
}
