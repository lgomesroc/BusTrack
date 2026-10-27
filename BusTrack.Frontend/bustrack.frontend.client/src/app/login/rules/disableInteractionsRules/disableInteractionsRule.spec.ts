import { disableInteractionsRule, disableRightClick } from './disableInteractionsRule';

describe('disableInteractionsRule', () => {
  beforeEach(() => {
    disableInteractionsRule();
  });

  it('prevents copy, cut, paste, context menu and selection', () => {
    for (const type of ['copy', 'cut', 'paste', 'contextmenu', 'selectstart']) {
      const event = new Event(type, { cancelable: true });
      const preventDefault = spyOn(event, 'preventDefault').and.callThrough();
      document.dispatchEvent(event);
      expect(preventDefault).toHaveBeenCalled();
    }
  });

  it('prevents arrow-key navigation', () => {
    for (const key of ['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight']) {
      const event = new KeyboardEvent('keydown', { key, cancelable: true });
      const preventDefault = spyOn(event, 'preventDefault').and.callThrough();
      document.dispatchEvent(event);
      expect(preventDefault).toHaveBeenCalled();
    }
  });

  it('disableRightClick prevents the context menu', () => {
    disableRightClick();
    const event = new MouseEvent('contextmenu', { cancelable: true });
    const preventDefault = spyOn(event, 'preventDefault').and.callThrough();

    document.dispatchEvent(event);

    expect(preventDefault).toHaveBeenCalled();
  });
});
