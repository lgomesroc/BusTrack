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
  user: any; 
  constructor(private dataService: DataService) { } 

  ngOnInit(): void {
    blockSavePasswordRule();
    disableInteractionsRule();
    disableRightClick();
    disableKeyboardShortcutsRule();
    preventBackNavigationRule();

    const emailInput = document.getElementById('email') as HTMLInputElement;
    validateEmailFormatRule(emailInput.value);

    const requiredField = document.getElementById('requiredField') as HTMLInputElement;
    validateFieldsRequiredRule(requiredField); 

    const characterLimitField = document.getElementById('characterLimitField') as HTMLInputElement;
    limitCharactersRule(characterLimitField, 200);

    const inactivityTimeout = 600000; 
    startInactivityTimerRule(inactivityTimeout);

    const sessionTimeout = 1800000; 
    startSessionTimeoutRule(sessionTimeout);

    window.onunload = () => {
      clearInactivityTimerRule();
      clearSessionTimeoutRule();
    };

    this.user = {
      name: 'Nome do Usuário',
      id: 'ID do Usuário'
    };
  }


  saveData() {
    const formData = {
      lineNumber: (document.getElementById('lineNumber') as HTMLInputElement).value,
      origin: (document.getElementById('origin') as HTMLInputElement).value,
      destination: (document.getElementById('destination') as HTMLInputElement).value,
      departureTime: (document.getElementById('departureTime') as HTMLInputElement).value,
      passengerAverage: (document.getElementById('passengerAverage') as HTMLInputElement).value
    };

    this.dataService.saveDataToMongoDB(formData).subscribe(response => {
      console.log('Dados salvos com sucesso:', response);
    }, error => {
      console.error('Erro ao salvar os dados:', error);
    });
  }
}
