export function preventForwardNavigationRule(): void {
  window.addEventListener('beforeunload', function (e) {
    const confirmationMessage = 'Deseja sair desta página?';
    (e || window.event).returnValue = confirmationMessage;
    return confirmationMessage;
  });
}
