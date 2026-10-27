
import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, Router, UrlTree } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { DashboardRoleGuard } from './dashboard-role.guard';
import { SessionService } from '../services/session.service';

describe('DashboardRoleGuard', () => {
  let guard: DashboardRoleGuard;
  let router: Router;
  let session: jasmine.SpyObj<SessionService>;

  function criarSnapshot(data: Record<string, unknown>): ActivatedRouteSnapshot {
    return { data } as unknown as ActivatedRouteSnapshot;
  }

  function esperarRedirecionamento(
    resultado: boolean | UrlTree,
    destino: string
  ): void {
    expect(resultado).toEqual(jasmine.any(UrlTree));
    expect(router.serializeUrl(resultado as UrlTree)).toBe(destino);
  }

  beforeEach(() => {
    session = jasmine.createSpyObj<SessionService>(
      'SessionService',
      ['isAuthenticated', 'isAdmin']
    );

    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        DashboardRoleGuard,
        { provide: SessionService, useValue: session }
      ]
    });

    guard = TestBed.inject(DashboardRoleGuard);
    router = TestBed.inject(Router);
  });

  it('redireciona usuários não autenticados para o login', () => {
    session.isAuthenticated.and.returnValue(false);

    const resultado = guard.canActivate(criarSnapshot({}));

    esperarRedirecionamento(resultado, '/enter-the-system');
    expect(session.isAdmin).not.toHaveBeenCalled();
  });

  it('redireciona administradores de /dashboard para o painel administrativo', () => {
    session.isAuthenticated.and.returnValue(true);
    session.isAdmin.and.returnValue(true);

    const resultado = guard.canActivate(
      criarSnapshot({ redirectByRole: true })
    );

    esperarRedirecionamento(resultado, '/admin-dashboard');
  });

  it('redireciona usuários comuns de /dashboard para o painel do usuário', () => {
    session.isAuthenticated.and.returnValue(true);
    session.isAdmin.and.returnValue(false);

    const resultado = guard.canActivate(
      criarSnapshot({ redirectByRole: true })
    );

    esperarRedirecionamento(resultado, '/user-dashboard');
  });

  it('impede usuários comuns de acessar o painel administrativo', () => {
    session.isAuthenticated.and.returnValue(true);
    session.isAdmin.and.returnValue(false);

    const resultado = guard.canActivate(
      criarSnapshot({ role: 'ADMIN' })
    );

    esperarRedirecionamento(resultado, '/user-dashboard');
  });

  it('impede administradores de acessar o painel do usuário', () => {
    session.isAuthenticated.and.returnValue(true);
    session.isAdmin.and.returnValue(true);

    const resultado = guard.canActivate(
      criarSnapshot({ role: 'USER' })
    );

    esperarRedirecionamento(resultado, '/admin-dashboard');
  });

  it('permite que administradores acessem o painel administrativo', () => {
    session.isAuthenticated.and.returnValue(true);
    session.isAdmin.and.returnValue(true);

    expect(
      guard.canActivate(criarSnapshot({ role: 'ADMIN' }))
    ).toBeTrue();
  });

  it('permite que usuários comuns acessem o painel do usuário', () => {
    session.isAuthenticated.and.returnValue(true);
    session.isAdmin.and.returnValue(false);

    expect(
      guard.canActivate(criarSnapshot({ role: 'USER' }))
    ).toBeTrue();
  });
});
