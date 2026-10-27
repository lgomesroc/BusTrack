import { validateFieldsRequiredRule } from './validateFieldsRequiredRule';

describe('validateFieldsRequiredRule', () => {
  let campo1: HTMLInputElement;
  let campo2: HTMLInputElement;
  let requiredField: HTMLInputElement;
  let alertSpy: jasmine.Spy;

  beforeEach(() => {
    document.body.innerHTML = `
      <input id="campo1">
      <input id="campo2">
      <input id="required">
    `;
    campo1 = document.getElementById('campo1') as HTMLInputElement;
    campo2 = document.getElementById('campo2') as HTMLInputElement;
    requiredField = document.getElementById('required') as HTMLInputElement;
    alertSpy = spyOn(window, 'alert');
  });

  it('returns true when all required fields are filled', () => {
    campo1.value = 'one';
    campo2.value = 'two';
    requiredField.value = 'three';

    expect(validateFieldsRequiredRule(requiredField)).toBeTrue();
    expect(alertSpy).not.toHaveBeenCalled();
  });

  it('returns false when any required field is empty', () => {
    campo1.value = 'one';
    campo2.value = '';
    requiredField.value = 'three';

    expect(validateFieldsRequiredRule(requiredField)).toBeFalse();
    expect(alertSpy).toHaveBeenCalledWith('Por favor, preencha todos os campos obrigatórios.');
  });
});
