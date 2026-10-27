import { blockSavePasswordRule } from './blockSavePasswordRule';

describe('blockSavePasswordRule', () => {
  afterEach(() => {
    document.body.innerHTML = '';
  });

  it('disables autocomplete on existing forms and text-like inputs', () => {
    document.body.innerHTML = `
      <form autocomplete="on"></form>
      <input type="password" autocomplete="on">
      <input type="email" autocomplete="on">
      <input type="text" autocomplete="on">
      <input type="number" autocomplete="on">
    `;

    blockSavePasswordRule();

    expect(document.querySelector('form')?.getAttribute('autocomplete')).toBe('off');
    expect(document.querySelector('input[type="password"]')?.getAttribute('autocomplete')).toBe('off');
    expect(document.querySelector('input[type="email"]')?.getAttribute('autocomplete')).toBe('off');
    expect(document.querySelector('input[type="text"]')?.getAttribute('autocomplete')).toBe('off');
    expect(document.querySelector('input[type="number"]')?.getAttribute('autocomplete')).toBe('on');
  });

  it('applies the rule to dynamically added forms and inputs', async () => {
    blockSavePasswordRule();

    const form = document.createElement('form');
    form.setAttribute('autocomplete', 'on');
    const input = document.createElement('input');
    input.type = 'password';
    input.setAttribute('autocomplete', 'on');
    form.appendChild(input);
    document.body.appendChild(form);

    await Promise.resolve();

    expect(form.getAttribute('autocomplete')).toBe('off');
    expect(input.getAttribute('autocomplete')).toBe('off');
  });
});
