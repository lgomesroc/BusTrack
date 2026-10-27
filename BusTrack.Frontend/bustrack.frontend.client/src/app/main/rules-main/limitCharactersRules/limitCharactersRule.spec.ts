import { limitCharactersRule } from './limitCharactersRule';

describe('limitCharactersRule', () => {
  afterEach(() => {
    document.body.innerHTML = '';
  });

  it('truncates input values above the configured limit', () => {
    const input = document.createElement('input');
    input.value = 'abcdef';
    const alertSpy = spyOn(window, 'alert');
    document.body.appendChild(input);

    limitCharactersRule(input, 3);
    input.dispatchEvent(new Event('input'));

    expect(input.value).toBe('abc');
    expect(alertSpy).toHaveBeenCalledWith('O campo excede o limite de caracteres.');
  });

  it('keeps values within the limit unchanged', () => {
    const input = document.createElement('input');
    input.value = 'abc';

    limitCharactersRule(input, 3);
    input.dispatchEvent(new Event('input'));

    expect(input.value).toBe('abc');
  });
});
