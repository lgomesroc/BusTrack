export function disableKeyboardShortcutsRule(): void {
  document.addEventListener('copy', (event: ClipboardEvent) => {
    event.preventDefault();
    alert('Copiar conteúdo desta página está desativado.');
  });

  document.addEventListener('cut', (event: ClipboardEvent) => {
    event.preventDefault();
    alert('Cortar conteúdo desta página está desativado.');
  });

  document.addEventListener('paste', (event: ClipboardEvent) => {
    event.preventDefault();
    alert('Colar conteúdo nesta página está desativado.');
  });

  document.addEventListener('contextmenu', (event: MouseEvent) => {
    event.preventDefault();
    alert('O menu de contexto está desativado.');
  });

  document.addEventListener('selectstart', (event: Event) => {
    event.preventDefault();
    alert('Selecionar texto nesta página está desativado.');
  });

  document.addEventListener('keydown', (event: KeyboardEvent) => {
    // Impede atalhos de teclado (Ctrl+C, Ctrl+V, etc.)
    if (event.ctrlKey && ['c', 'v', 'x', 'p'].includes(event.key)) {
      event.preventDefault();
      alert('Atalhos de teclado estão desativados.');
    }
  });
}
