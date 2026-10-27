import { BlockCopyRule } from './blockCopyRule';

describe('blockCopyRule', () => {
  beforeEach(() => {
    BlockCopyRule();
  });

  it('prevents copying content', () => {
    const event = new Event('copy', { cancelable: true });
    const preventDefault = spyOn(event, 'preventDefault').and.callThrough();

    document.dispatchEvent(event);

    expect(preventDefault).toHaveBeenCalled();
  });

  it('prevents the context menu', () => {
    const event = new MouseEvent('contextmenu', { cancelable: true });
    const preventDefault = spyOn(event, 'preventDefault').and.callThrough();

    document.dispatchEvent(event);

    expect(preventDefault).toHaveBeenCalled();
  });
});
