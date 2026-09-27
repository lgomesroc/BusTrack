export function blockSavePasswordRule(): void {

  const aplicarRegra = (): void => {

    const forms =
      document.querySelectorAll('form');

    forms.forEach(form => {

      form.setAttribute(
        'autocomplete',
        'off'
      );

    });


    const inputs =
      document.querySelectorAll('input');

    inputs.forEach(input => {

      const type =
        input.getAttribute('type')?.toLowerCase();

      if (
        type === 'password' ||
        type === 'email' ||
        type === 'text'
      ) {

        input.setAttribute(
          'autocomplete',
          'off'
        );

      }

    });

  };


  aplicarRegra();


  const observer =
    new MutationObserver(() => {

      aplicarRegra();

    });


  observer.observe(
    document.body,
    {
      childList: true,
      subtree: true
    }
  );

}
