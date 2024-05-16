export function validateFieldsRequiredRule(requiredField: HTMLInputElement): boolean {
  const campo1 = document.getElementById('campo1') as HTMLInputElement;
  const campo2 = document.getElementById('campo2') as HTMLInputElement;

  if (campo1.value === '' || campo2.value === '' || requiredField.value === '') {
    alert('Por favor, preencha todos os campos obrigatórios.');
    return false;
  }

  return true;
}
