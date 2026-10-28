let sessionTimeoutRule: ReturnType<typeof setTimeout> | undefined;

export function startSessionTimeoutRule(
  timeout: number,
  redirect: (url: string) => void = url => {
    window.location.href = url;
  }
): void {
  clearSessionTimeoutRule();

  sessionTimeoutRule = setTimeout(() => {
    redirect('/enter-the-system');
    sessionTimeoutRule = undefined;
  }, timeout);
}

export function clearSessionTimeoutRule(): void {
  if (sessionTimeoutRule !== undefined) {
    clearTimeout(sessionTimeoutRule);
    sessionTimeoutRule = undefined;
  }
}
