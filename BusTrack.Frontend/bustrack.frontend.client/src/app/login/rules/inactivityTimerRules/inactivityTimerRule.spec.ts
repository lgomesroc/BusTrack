import {
  clearInactivityTimerRule,
  startInactivityTimerRule
} from './inactivityTimerRule';

describe('inactivityTimerRule', () => {
  beforeEach(() => {
    jasmine.clock().install();
    jasmine.clock().mockDate(new Date('2026-01-01T00:00:00Z'));
    clearInactivityTimerRule();
  });

  afterEach(() => {
    clearInactivityTimerRule();
    jasmine.clock().uninstall();
  });

  it('calls the expiration callback after the inactivity duration', () => {
    const onTimeout = jasmine.createSpy('onTimeout');

    startInactivityTimerRule(20 * 60 * 1000, onTimeout);
    jasmine.clock().tick(20 * 60 * 1000);

    expect(onTimeout).toHaveBeenCalledTimes(1);
  });

  it('resets the timer when mouse, keyboard, and click activity occurs', () => {
    const onTimeout = jasmine.createSpy('onTimeout');

    startInactivityTimerRule(1000, onTimeout);
    jasmine.clock().tick(700);
    window.dispatchEvent(new MouseEvent('mousemove'));
    jasmine.clock().tick(700);

    expect(onTimeout).not.toHaveBeenCalled();

    window.dispatchEvent(new KeyboardEvent('keydown', { key: 'a' }));
    jasmine.clock().tick(700);
    window.dispatchEvent(new MouseEvent('click'));
    jasmine.clock().tick(999);

    expect(onTimeout).not.toHaveBeenCalled();
    jasmine.clock().tick(1);
    expect(onTimeout).toHaveBeenCalledTimes(1);
  });

  it('cancels the timer and removes listeners when cleared', () => {
    const onTimeout = jasmine.createSpy('onTimeout');

    startInactivityTimerRule(1000, onTimeout);
    clearInactivityTimerRule();
    jasmine.clock().tick(1000);

    expect(onTimeout).not.toHaveBeenCalled();
  });

  it('checks elapsed time when the hidden tab becomes visible', () => {
    const onTimeout = jasmine.createSpy('onTimeout');

    startInactivityTimerRule(1000, onTimeout);
    jasmine.clock().tick(500);
    jasmine.clock().mockDate(new Date(Date.now() + 600));
    document.dispatchEvent(new Event('visibilitychange'));

    expect(onTimeout).toHaveBeenCalledTimes(1);
  });

  it('does not invoke the callback more than once after expiration', () => {
    const onTimeout = jasmine.createSpy('onTimeout');

    startInactivityTimerRule(1000, onTimeout);
    jasmine.clock().tick(3000);

    expect(onTimeout).toHaveBeenCalledTimes(1);
  });
});
