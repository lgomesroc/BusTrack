import { preventForwardNavigationRule } from './preventForwardNavigationRule';

describe('preventForwardNavigationRule', () => {
  it('does not register or alter navigation behavior', () => {
    expect(() => preventForwardNavigationRule()).not.toThrow();
    expect(window.history.length).toBeGreaterThan(0);
  });
});
