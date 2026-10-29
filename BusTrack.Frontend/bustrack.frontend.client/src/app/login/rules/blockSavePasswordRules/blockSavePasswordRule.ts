export function blockSavePasswordRule(): () => void {
  const aplicarRegra = (): void => {
    document.querySelectorAll('form').forEach(form => {
      form.setAttribute('autocomplete', 'off');
    });

    document.querySelectorAll('input').forEach(input => {
      const type = input.getAttribute('type')?.toLowerCase();

      if (
        type === 'password' ||
        type === 'email' ||
        type === 'text'
      ) {
        input.setAttribute('autocomplete', 'off');
      }
    });
  };

  aplicarRegra();

  const observer = new MutationObserver(() => {
    aplicarRegra();
  });

  if (document.body) {
    observer.observe(document.body, {
      childList: true,
      subtree: true
    });
  }

  return () => {
    observer.disconnect();
  };
}
