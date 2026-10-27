import { validateEmailFormatRule } from './validateEmailFormatRule';

describe('validateEmailFormatRule', () => {
  it('accepts valid email formats', () => {
    expect(validateEmailFormatRule('user@example.com')).toBeTrue();
    expect(validateEmailFormatRule('user.name+tag@example.com')).toBeTrue();
  });

  it('rejects invalid email formats', () => {
    expect(validateEmailFormatRule('user')).toBeFalse();
    expect(validateEmailFormatRule('user@')).toBeFalse();
    expect(validateEmailFormatRule('@example.com')).toBeFalse();
    expect(validateEmailFormatRule('user @example.com')).toBeFalse();
  });
});
