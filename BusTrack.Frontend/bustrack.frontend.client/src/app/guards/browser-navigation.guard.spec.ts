import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { BrowserNavigationGuard } from './browser-navigation.guard';

describe('BrowserNavigationGuard', () => {
  let guard: BrowserNavigationGuard;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    router = jasmine.createSpyObj<Router>(
      'Router',
      ['getCurrentNavigation']
    );

    TestBed.configureTestingModule({
      providers: [
        BrowserNavigationGuard,
        { provide: Router, useValue: router }
      ]
    });

    guard = TestBed.inject(BrowserNavigationGuard);
  });

  afterEach(() => {
    if (window.alert && (window.alert as jasmine.Spy).and) {
      (window.alert as jasmine.Spy).and.stub();
    }
  });

  it('allows normal router navigation', () => {
    router.getCurrentNavigation.and.returnValue({
      trigger: 'imperative'
    } as ReturnType<Router['getCurrentNavigation']>);

    expect(guard.canActivate()).toBeTrue();
  });

  it('blocks browser back/forward navigation and warns the user', () => {
    spyOn(window, 'alert');
    router.getCurrentNavigation.and.returnValue({
      trigger: 'popstate'
    } as ReturnType<Router['getCurrentNavigation']>);

    expect(guard.canActivate()).toBeFalse();
    expect(window.alert).toHaveBeenCalledWith(
      'A navegação para trás e para frente pelo navegador está desativada.'
    );
  });

  it('allows navigation when there is no current navigation', () => {
    router.getCurrentNavigation.and.returnValue(null);

    expect(guard.canActivate()).toBeTrue();
  });
});
