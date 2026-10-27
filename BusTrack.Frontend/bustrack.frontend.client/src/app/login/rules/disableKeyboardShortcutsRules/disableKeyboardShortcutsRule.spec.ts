import { disableKeyboardShortcutsRule } from './disableKeyboardShortcutsRule';

describe('disableKeyboardShortcutsRule', () => {
  beforeEach(() => {
    disableKeyboardShortcutsRule();
  });

  it('prevents cut, paste and text selection', () => {
    for (const type of ['cut', 'paste', 'selectstart']) {
      const event = new Event(type, { cancelable: true });
      const preventDefault = spyOn(event, 'preventDefault').and.callThrough();
      document.dispatchEvent(event);
      expect(preventDefault).toHaveBeenCalled();
    }
  });

  it('prevents configured control shortcuts', () => {
    for (const key of ['c', 'v', 'x', 'p']) {
      const event = new KeyboardEvent('keydown', { key, ctrlKey: true, cancelable: true });
      const preventDefault = spyOn(event, 'preventDefault').and.callThrough();
      document.dispatchEvent(event);
      expect(preventDefault).toHaveBeenCalled();
    }
  });

  it('does not block unrelated keyboard input', () => {
    const event = new KeyboardEvent('keydown', { key: 'a', cancelable: true });
    const preventDefault = spyOn(event, 'preventDefault').and.callThrough();

    document.dispatchEvent(event);

    expect(preventDefault).not.toHaveBeenCalled();
  });
});
