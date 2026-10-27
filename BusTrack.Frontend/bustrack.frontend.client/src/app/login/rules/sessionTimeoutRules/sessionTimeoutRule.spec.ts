import {
  clearSessionTimeoutRule,
  startSessionTimeoutRule
} from './sessionTimeoutRule';

describe('sessionTimeoutRule', () => {
  beforeEach(() => {
    jasmine.clock().install();
  });

  afterEach(() => {
    clearSessionTimeoutRule();
    jasmine.clock().uninstall();
  });

  it('redirects to the login screen after the configured timeout', () => {
    const redirect = jasmine.createSpy('redirect');

    startSessionTimeoutRule(1000, redirect);

    expect(redirect).not.toHaveBeenCalled();

    jasmine.clock().tick(999);
    expect(redirect).not.toHaveBeenCalled();

    jasmine.clock().tick(1);

    expect(redirect).toHaveBeenCalledOnceWith('/enter-the-system');
  });

  it('cancels the scheduled timeout', () => {
    const redirect = jasmine.createSpy('redirect');

    startSessionTimeoutRule(1000, redirect);
    clearSessionTimeoutRule();

    jasmine.clock().tick(1000);

    expect(redirect).not.toHaveBeenCalled();
  });

  it('replaces a previously scheduled timeout', () => {
    const firstRedirect = jasmine.createSpy('firstRedirect');
    const secondRedirect = jasmine.createSpy('secondRedirect');

    startSessionTimeoutRule(1000, firstRedirect);
    startSessionTimeoutRule(2000, secondRedirect);

    jasmine.clock().tick(1000);

    expect(firstRedirect).not.toHaveBeenCalled();
    expect(secondRedirect).not.toHaveBeenCalled();

    jasmine.clock().tick(1000);

    expect(firstRedirect).not.toHaveBeenCalled();
    expect(secondRedirect).toHaveBeenCalledOnceWith('/enter-the-system');
  });
});
