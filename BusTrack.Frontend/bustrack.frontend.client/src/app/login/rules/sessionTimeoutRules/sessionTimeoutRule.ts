let sessionTimeoutRule: any;

export function startSessionTimeoutRule(timeout: number): void {
  sessionTimeoutRule = setTimeout(() => {
        window.location.href = '/enter-the-system';
    }, timeout);
}

export function clearSessionTimeoutRule(): void {
  clearTimeout(sessionTimeoutRule);
}
