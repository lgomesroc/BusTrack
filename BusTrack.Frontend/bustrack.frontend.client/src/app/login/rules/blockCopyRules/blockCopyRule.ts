export function BlockCopyRule(): void {
  document.addEventListener('copy', (event: ClipboardEvent) => {
    event.preventDefault();
    alert('Copiar conteúdo desta página está desativado.');
  });

  document.addEventListener('contextmenu', (event: MouseEvent) => {
    event.preventDefault();
    alert('O menu de contexto está desativado.');
  });
}

