// preventBackNavigationRule.ts
export function preventBackNavigationRule(): void {
  window.addEventListener('popstate', () => {
    history.pushState(null, document.title, location.href);
    alert('Navegação para trás está desativada.');
  });

  history.pushState(null, document.title, location.href);
}
