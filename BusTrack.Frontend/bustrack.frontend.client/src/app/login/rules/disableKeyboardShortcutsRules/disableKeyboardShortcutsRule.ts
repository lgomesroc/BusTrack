let ruleInitialized = false;

function handleCut(event: ClipboardEvent): void {
  event.preventDefault();

  alert(
    'Cortar conteúdo desta página está desativado.'
  );
}

function handlePaste(event: ClipboardEvent): void {
  event.preventDefault();

  alert(
    'Colar conteúdo nesta página está desativado.'
  );
}

function handleSelectStart(event: Event): void {
  event.preventDefault();

  alert(
    'Selecionar texto nesta página está desativado.'
  );
}

function handleKeyDown(event: KeyboardEvent): void {
  if (
    event.ctrlKey &&
    ['c', 'v', 'x', 'p'].includes(
      event.key.toLowerCase()
    )
  ) {
    event.preventDefault();

    alert(
      'Atalhos de teclado estão desativados.'
    );
  }
}

export function disableKeyboardShortcutsRule(): void {
  if (ruleInitialized) {
    return;
  }

  document.addEventListener(
    'cut',
    handleCut
  );

  document.addEventListener(
    'paste',
    handlePaste
  );

  document.addEventListener(
    'selectstart',
    handleSelectStart
  );

  document.addEventListener(
    'keydown',
    handleKeyDown
  );

  ruleInitialized = true;
}
