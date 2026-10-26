let inactivityTimeout: number | undefined;
let ruleInitialized = false;
let currentInactivityDuration = 0;
let lastActivityAt = 0;
let onInactivity: (() => void) | undefined;

function recordActivity(): void {
  lastActivityAt = Date.now();
  resetInactivityTimer();
}

function handleVisibilityChange(): void {
  if (document.visibilityState === 'visible') {
    checkInactivity();
  }
}

function checkInactivity(): void {
  const elapsed = Date.now() - lastActivityAt;
  const remaining = currentInactivityDuration - elapsed;

  if (remaining <= 0) {
    expireSession();
    return;
  }

  scheduleExpiration(remaining);
}

function scheduleExpiration(delay: number): void {
  if (inactivityTimeout !== undefined) {
    window.clearTimeout(inactivityTimeout);
  }

  inactivityTimeout = window.setTimeout(checkInactivity, delay);
}

function expireSession(): void {
  clearInactivityTimerRule();
  const callback = onInactivity;
  onInactivity = undefined;
  callback?.();
}

export function startInactivityTimerRule(
  inactivityTimeoutDuration: number,
  onTimeout?: () => void
): void {
  currentInactivityDuration = inactivityTimeoutDuration;
  onInactivity = onTimeout;

  if (ruleInitialized) {
    checkInactivity();
    return;
  }

  lastActivityAt = Date.now();

  window.addEventListener('mousemove', recordActivity);
  window.addEventListener('keydown', recordActivity);
  window.addEventListener('click', recordActivity);
  window.addEventListener('scroll', recordActivity, true);
  window.addEventListener('touchstart', recordActivity, { passive: true });
  document.addEventListener('visibilitychange', handleVisibilityChange);

  ruleInitialized = true;
  resetInactivityTimer();
}

export function clearInactivityTimerRule(): void {
  if (inactivityTimeout !== undefined) {
    window.clearTimeout(inactivityTimeout);
    inactivityTimeout = undefined;
  }

  window.removeEventListener('mousemove', recordActivity);
  window.removeEventListener('keydown', recordActivity);
  window.removeEventListener('click', recordActivity);
  window.removeEventListener('scroll', recordActivity, true);
  window.removeEventListener('touchstart', recordActivity);
  document.removeEventListener('visibilitychange', handleVisibilityChange);

  ruleInitialized = false;
}

function resetInactivityTimer(): void {
  scheduleExpiration(currentInactivityDuration);
}
