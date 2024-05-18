// inactivityTimerRule.ts
let inactivityTimeout: number; // Corrigido para 'number' para compatibilidade com o navegador
// Cria um mapa para armazenar os ouvintes de eventos
const eventListeners: { [key: string]: EventListener } = {};

export function startInactivityTimerRule(inactivityTimeoutDuration: number): void {
  // Define os ouvintes de eventos e os armazena no mapa
  eventListeners['mouseMoveListener'] = () => resetInactivityTimer(inactivityTimeoutDuration);
  eventListeners['keydownListener'] = () => resetInactivityTimer(inactivityTimeoutDuration);

  window.addEventListener('mousemove', eventListeners['mouseMoveListener']);
  window.addEventListener('keydown', eventListeners['keydownListener']);
}

export function clearInactivityTimerRule(): void {
  window.clearTimeout(inactivityTimeout); // Use 'window.clearTimeout' para clareza

  // Usa as referências armazenadas no mapa para remover os ouvintes
  window.removeEventListener('mousemove', eventListeners['mouseMoveListener']);
  window.removeEventListener('keydown', eventListeners['keydownListener']);
}

function resetInactivityTimer(inactivityTimeoutDuration: number): void {
  window.clearTimeout(inactivityTimeout); // Use 'window.clearTimeout' para clareza
  inactivityTimeout = window.setTimeout(() => { // Use 'window.setTimeout' para clareza
    alert('Você foi desconectado por inatividade.');
    // Adicionar lógica para desconectar o usuário ou redirecionar
  }, inactivityTimeoutDuration);
}
