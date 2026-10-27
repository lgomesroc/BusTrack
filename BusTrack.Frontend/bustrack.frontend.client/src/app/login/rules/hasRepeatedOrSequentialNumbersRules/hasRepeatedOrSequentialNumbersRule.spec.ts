import { hasRepeatedOrSequentialNumbersRule } from './hasRepeatedOrSequentialNumbersRule';

describe('hasRepeatedOrSequentialNumbersRule', () => {
  it('detects repeated adjacent characters', () => {
    expect(hasRepeatedOrSequentialNumbersRule('aa')).toBe(false);
    expect(hasRepeatedOrSequentialNumbersRule('aaa')).toBe(true);
  });

  it('detects a repeated character separated by one character', () => {
    expect(hasRepeatedOrSequentialNumbersRule('aba')).toBe(true);
  });

  it('detects sequential digits', () => {
    expect(hasRepeatedOrSequentialNumbersRule('12abc')).toBe(true);
    expect(hasRepeatedOrSequentialNumbersRule('90abc')).toBe(false);
  });

  it('ignores non-numeric characters when checking digit sequences', () => {
    expect(hasRepeatedOrSequentialNumbersRule('a12b')).toBe(true);
    expect(hasRepeatedOrSequentialNumbersRule('a1b2')).toBe(false);
  });

  it('returns false when no repeated or sequential pattern exists', () => {
    expect(hasRepeatedOrSequentialNumbersRule('Ab7x')).toBe(false);
  });
});
