let ruleInitialized = false;

function handleCopy(event: ClipboardEvent): void {
  event.preventDefault();

  alert(
    'Copiar conteúdo desta página está desativado.'
  );
}

function handleContextMenu(event: MouseEvent): void {
  event.preventDefault();

  alert(
    'O menu de contexto está desativado.'
  );
}

export function BlockCopyRule(): void {
  if (ruleInitialized) {
    return;
  }

  document.addEventListener(
    'copy',
    handleCopy
  );

  document.addEventListener(
    'contextmenu',
    handleContextMenu
  );

  ruleInitialized = true;
}
