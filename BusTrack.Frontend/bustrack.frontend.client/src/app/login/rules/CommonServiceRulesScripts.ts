import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CommonServiceRulesScripts {
  private inactivityTimer: any;

  constructor() { }

  // Método para desabilitar atalhos do teclado
  disableKeyboardShortcuts(): void {
    document.addEventListener('copy', (event) => event.preventDefault());
    document.addEventListener('cut', (event) => event.preventDefault());
    document.addEventListener('paste', (event) => event.preventDefault());

    document.addEventListener('contextmenu', (event) => event.preventDefault());

    document.addEventListener('selectstart', (event) => event.preventDefault());

    document.addEventListener('keydown', (event) => {
      if (event.key === 'ArrowUp' || event.key === 'ArrowDown' || event.key === 'ArrowLeft' || event.key === 'ArrowRight') {
        event.preventDefault();
      }
    });
  }

  // Método para iniciar o temporizador de inatividade
  startInactivityTimer(timeout: number): void {
    this.inactivityTimer = setTimeout(() => {
      // Ação a ser realizada quando o temporizador de inatividade expirar
      window.location.reload();
    }, timeout);
  }

  // Método para limpar o temporizador de inatividade
  clearInactivityTimer(): void {
    clearTimeout(this.inactivityTimer);
  }

  // Impedir o usuário de voltar para a tela anterior
  preventBackNavigation(): void {
    window.history.pushState(null, '', window.location.href);
    window.onpopstate = function () {
      window.history.go(1);
    };
  }

  // Impedir o usuário de ir para a próxima página
  preventForwardNavigation(): void {
    window.addEventListener('beforeunload', function (e) {
      const confirmationMessage = 'Deseja sair desta página?';
      (e || window.event).returnValue = confirmationMessage;
      return confirmationMessage;
    });
  }
}

