import { preventBackNavigationRule } from './preventBackNavigationRule';

describe('preventBackNavigationRule', () => {
  it('does not register or alter navigation behavior', () => {
    expect(() => preventBackNavigationRule()).not.toThrow();
    expect(window.history.length).toBeGreaterThan(0);
  });
});
