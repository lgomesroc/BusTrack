export function BlockCopyRule(): void {
  // Bloqueia a cópia de texto
  document.addEventListener('copy', (event: ClipboardEvent) => {
    event.preventDefault();
    alert('Copiar conteúdo desta página está desativado.');
  });

  // Bloqueia o menu de contexto do mouse
  document.addEventListener('contextmenu', (event: MouseEvent) => {
    event.preventDefault();
    alert('O menu de contexto está desativado.');
  });
}

