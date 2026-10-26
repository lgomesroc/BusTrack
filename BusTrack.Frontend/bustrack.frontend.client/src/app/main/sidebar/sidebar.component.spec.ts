import { CommonModule } from '@angular/common';
import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { SessionService } from '../../services/session.service';
import { SidebarComponent } from './sidebar.component';

describe('SidebarComponent', () => {
  let sessionService: jasmine.SpyObj<SessionService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    sessionService = jasmine.createSpyObj<SessionService>(
      'SessionService',
      ['getAccessToken', 'clearSession']
    );
    sessionService.getAccessToken.and.returnValue(null);
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [CommonModule],
      declarations: [SidebarComponent],
      providers: [
        { provide: SessionService, useValue: sessionService },
        { provide: Router, useValue: router }
      ]
    }).compileComponents();
  });

  afterEach(() => {
    TestBed.resetTestingModule();
  });

  it('starts the 20-minute timer and logs out after the timeout', fakeAsync(() => {
    const fixture = TestBed.createComponent(SidebarComponent);
    fixture.detectChanges();

    tick(19 * 60 * 1000);
    expect(sessionService.clearSession).not.toHaveBeenCalled();

    tick(60 * 1000);
    expect(sessionService.clearSession).toHaveBeenCalledTimes(1);
    expect(router.navigate).toHaveBeenCalledWith(
      ['/enter-the-system'],
      { replaceUrl: true }
    );

    fixture.destroy();
  }));

  it('clears the inactivity timer when the sidebar is destroyed', fakeAsync(() => {
    const fixture = TestBed.createComponent(SidebarComponent);
    fixture.detectChanges();
    fixture.destroy();

    tick(20 * 60 * 1000);

    expect(sessionService.clearSession).not.toHaveBeenCalled();
    expect(router.navigate).not.toHaveBeenCalled();
  }));

  it('updates the displayed user data from a valid access token', () => {
    const payload = btoa(JSON.stringify({
      name: 'Usuario de teste',
      email: 'teste@example.com',
      role: 'ADMIN'
    }));
    sessionService.getAccessToken.and.returnValue(`header.${payload}.signature`);

    const fixture = TestBed.createComponent(SidebarComponent);
    fixture.detectChanges();

    expect(fixture.componentInstance.nomeUsuario).toBe('Usuario de teste');
    expect(fixture.componentInstance.emailUsuario).toBe('teste@example.com');
    expect(fixture.componentInstance.perfilUsuario).toBe('ADMIN');

    fixture.destroy();
  });
});
