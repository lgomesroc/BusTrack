let inactivityTimeout: number | undefined;

let ruleInitialized = false;

let currentInactivityDuration = 0;

function mouseMoveListener(): void {
  resetInactivityTimer(
    currentInactivityDuration
  );
}

function keyDownListener(): void {
  resetInactivityTimer(
    currentInactivityDuration
  );
}

export function startInactivityTimerRule(
  inactivityTimeoutDuration: number
): void {
  currentInactivityDuration =
    inactivityTimeoutDuration;

  if (ruleInitialized) {
    resetInactivityTimer(
      currentInactivityDuration
    );

    return;
  }

  window.addEventListener(
    'mousemove',
    mouseMoveListener
  );

  window.addEventListener(
    'keydown',
    keyDownListener
  );

  ruleInitialized = true;

  resetInactivityTimer(
    currentInactivityDuration
  );
}

export function clearInactivityTimerRule(): void {
  if (inactivityTimeout !== undefined) {
    window.clearTimeout(
      inactivityTimeout
    );

    inactivityTimeout = undefined;
  }

  window.removeEventListener(
    'mousemove',
    mouseMoveListener
  );

  window.removeEventListener(
    'keydown',
    keyDownListener
  );

  ruleInitialized = false;
}

function resetInactivityTimer(
  inactivityTimeoutDuration: number
): void {
  if (inactivityTimeout !== undefined) {
    window.clearTimeout(
      inactivityTimeout
    );
  }

  inactivityTimeout = window.setTimeout(
    () => {
      alert(
        'Você foi desconectado por inatividade.'
      );
    },
    inactivityTimeoutDuration
  );
}
