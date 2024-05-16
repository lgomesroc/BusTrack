let inactivityTimerRule: any;

export function startInactivityTimerRule(timeout: number): void {
  inactivityTimerRule = setTimeout(() => {
        // Ação a ser realizada quando o temporizador de inatividade expirar
        window.location.reload();
    }, timeout);
}

export function clearInactivityTimerRule(): void {
  clearTimeout(inactivityTimerRule);
}
