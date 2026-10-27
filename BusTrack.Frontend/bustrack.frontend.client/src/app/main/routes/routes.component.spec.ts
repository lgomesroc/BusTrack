import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { RoutesComponent } from './routes.component';
import { RouteService } from '../../services/route.service';
import { SessionService } from '../../services/session.service';
import { Route } from '../../services/trip.service';

describe('RoutesComponent', () => {
  let component: RoutesComponent;
  let fixture: ComponentFixture<RoutesComponent>;
  let routeService: jasmine.SpyObj<RouteService>;
  let sessionService: jasmine.SpyObj<SessionService>;
  let router: jasmine.SpyObj<Router>;

  const route: Route = {
    id: 'route-1',
    name: 'Centro',
    description: 'Rota de teste',
    origin: 'Origem',
    destination: 'Destino',
    distance: 15
  };

  beforeEach(async () => {
    routeService = jasmine.createSpyObj<RouteService>(
      'RouteService',
      ['getRoutes', 'createRoute', 'updateRoute', 'deleteRoute']
    );
    sessionService = jasmine.createSpyObj<SessionService>(
      'SessionService',
      ['isAdmin', 'isAuthenticated']
    );
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);
    routeService.getRoutes.and.returnValue(of([route]));

    await TestBed.configureTestingModule({
      declarations: [RoutesComponent],
      providers: [
        { provide: RouteService, useValue: routeService },
        { provide: SessionService, useValue: sessionService },
        { provide: Router, useValue: router }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(RoutesComponent);
    component = fixture.componentInstance;
  });

  it('creates and loads routes on initialization', () => {
    fixture.detectChanges();

    expect(component).toBeTruthy();
    expect(component.routes).toEqual([route]);
    expect(component.isLoading).toBeFalse();
  });

  it('rejects an incomplete route form', () => {
    component.startCreate();
    component.saveRoute();

    expect(routeService.createRoute).not.toHaveBeenCalled();
    expect(component.errorMessage).toContain('Preencha o nome');
  });

  it('rejects a negative distance', () => {
    component.startCreate();
    component.routeForm = { ...route, id: '', distance: -1 };

    component.saveRoute();

    expect(routeService.createRoute).not.toHaveBeenCalled();
    expect(component.errorMessage).toContain('distância deve ser um número inteiro');
  });

  it('creates a route when the form is valid', () => {
    routeService.createRoute.and.returnValue(of(route));
    component.startCreate();
    component.routeForm = { ...route, id: '' };

    component.saveRoute();

    expect(routeService.createRoute).toHaveBeenCalledWith({
      name: route.name,
      description: route.description,
      origin: route.origin,
      destination: route.destination,
      distance: route.distance
    });
    expect(component.routes).toContain(route);
    expect(component.isLoading).toBeFalse();
  });

  it('shows an error when routes fail to load', () => {
    routeService.getRoutes.and.returnValue(
      throwError(() => new Error('API indisponível'))
    );

    component.loadRoutes();

    expect(component.errorMessage).toContain('Não foi possível carregar as rotas');
    expect(component.isLoading).toBeFalse();
  });

  it('navigates back to the dashboard', () => {
    component.backToDashboard();

    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });
});
