import { checkPasswordStrengthRule } from './checkPasswordStrength(passwordRule';

describe('checkPasswordStrengthRule', () => {
  it('returns weak for short or simple passwords', () => {
    expect(checkPasswordStrengthRule('abc')).toBe('weak');
    expect(checkPasswordStrengthRule('abcdef')).toBe('weak');
  });

  it('returns medium for passwords meeting the medium requirements', () => {
    expect(checkPasswordStrengthRule('Abcdef')).toBe('medium');
    expect(checkPasswordStrengthRule('123456!')).toBe('medium');
  });

  it('returns strong for a complex password without repeated or sequential numbers', () => {
    expect(checkPasswordStrengthRule('Abcdef!9')).toBe('strong');
  });

  it('returns medium when a complex password contains a repeated or sequential number pattern', () => {
    expect(checkPasswordStrengthRule('Abcdef!12')).toBe('medium');
  });
});
