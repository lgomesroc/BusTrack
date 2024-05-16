export function blockSavePasswordRule(): void {
  const inputs = document.querySelectorAll('input[type="password"]');
  inputs.forEach(input => {
    input.setAttribute('autocomplete', 'off');
  });
}
