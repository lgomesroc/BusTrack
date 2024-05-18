// preventForwardNavigationRule.ts
export function preventForwardNavigationRule(): void {
  window.addEventListener('popstate', () => {
    history.pushState(null, document.title, location.href);
    alert('Navegação para frente está desativada.');
  });

  history.pushState(null, document.title, location.href);
}
