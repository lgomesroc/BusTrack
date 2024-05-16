export function disableKeyboardShortcutsRule(): void {
  document.addEventListener('copy', (event) => event.preventDefault());
  document.addEventListener('cut', (event) => event.preventDefault());
  document.addEventListener('paste', (event) => event.preventDefault());

  document.addEventListener('contextmenu', (event) => event.preventDefault());

  document.addEventListener('selectstart', (event) => event.preventDefault());

  document.addEventListener('keydown', (event: KeyboardEvent) => {
    if (event.key === 'ArrowUp' || event.key === 'ArrowDown' || event.key === 'ArrowLeft' || event.key === 'ArrowRight') {
      event.preventDefault();
    }
  });
}
