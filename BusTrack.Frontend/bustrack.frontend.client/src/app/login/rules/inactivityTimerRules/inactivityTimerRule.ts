let inactivityTimeout: number; 
const eventListeners: { [key: string]: EventListener } = {};

export function startInactivityTimerRule(inactivityTimeoutDuration: number): void {
  eventListeners['mouseMoveListener'] = () => resetInactivityTimer(inactivityTimeoutDuration);
  eventListeners['keydownListener'] = () => resetInactivityTimer(inactivityTimeoutDuration);

  window.addEventListener('mousemove', eventListeners['mouseMoveListener']);
  window.addEventListener('keydown', eventListeners['keydownListener']);
}

export function clearInactivityTimerRule(): void {
  window.clearTimeout(inactivityTimeout); 

  window.removeEventListener('mousemove', eventListeners['mouseMoveListener']);
  window.removeEventListener('keydown', eventListeners['keydownListener']);
}

function resetInactivityTimer(inactivityTimeoutDuration: number): void {
  window.clearTimeout(inactivityTimeout);
  inactivityTimeout = window.setTimeout(() => { 
    alert('Você foi desconectado por inatividade.');
  }, inactivityTimeoutDuration);
}
