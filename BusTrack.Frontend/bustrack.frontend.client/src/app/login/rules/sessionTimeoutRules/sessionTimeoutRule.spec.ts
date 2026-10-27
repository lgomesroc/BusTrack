import { clearSessionTimeoutRule, startSessionTimeoutRule } from './sessionTimeoutRule';

describe('sessionTimeoutRule', () => {
  beforeEach(() => {
    jasmine.clock().install();
  });

  afterEach(() => {
    clearSessionTimeoutRule();
    jasmine.clock().uninstall();
  });

  it('redirects to the login screen after the configured timeout', () => {
    const location = window.location;
    spyOnProperty(window, 'location', 'get').and.returnValue({ href: '' } as Location);

    startSessionTimeoutRule(1000);
    jasmine.clock().tick(1000);

    expect(window.location.href).toBe('/enter-the-system');
    void location;
  });

  it('cancels the scheduled timeout', () => {
    const locationSpy = spyOnProperty(window, 'location', 'get').and.returnValue({ href: '' } as Location);

    startSessionTimeoutRule(1000);
    clearSessionTimeoutRule();
    jasmine.clock().tick(1000);

    expect(locationSpy).not.toHaveBeenCalled();
  });
});
