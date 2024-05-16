let sessionTimeoutRule: any;

export function startSessionTimeoutRule(timeout: number): void {
  sessionTimeoutRule = setTimeout(() => {
        // Ação a ser realizada quando o timeout da sessão expirar
        // Exemplo: redirecionar para a tela de login
        window.location.href = '/login';
    }, timeout);
}

export function clearSessionTimeoutRule(): void {
  clearTimeout(sessionTimeoutRule);
}
