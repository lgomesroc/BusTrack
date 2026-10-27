import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { Router } from '@angular/router';
import { forkJoin, of, throwError } from 'rxjs';
import { TripsComponent } from './trips.component';
import { TripService, Trip } from '../../services/trip.service';
import { RouteService } from '../../services/route.service';
import { DriverService } from '../../services/driver.service';
import { SessionService } from '../../services/session.service';

describe('TripsComponent', () => {
  let component: TripsComponent;
  let fixture: ComponentFixture<TripsComponent>;
  let tripService: jasmine.SpyObj<TripService>;
  let routeService: jasmine.SpyObj<RouteService>;
  let driverService: jasmine.SpyObj<DriverService>;
  let sessionService: jasmine.SpyObj<SessionService>;
  let router: jasmine.SpyObj<Router>;

  const trip: Trip = {
    id: 'trip-1',
    bus: { id: 'bus-1', number: '101', licensePlate: 'ABC1D23', model: 'Modelo', capacity: 40 },
    driver: { id: 'driver-1', name: 'Motorista', licenseNumber: '123' },
    route: { id: 'route-1', name: 'Centro', description: '', origin: 'A', destination: 'B', distance: 10 },
    departureTime: '2099-10-20T10:00:00Z',
    startedAt: null,
    arrivalTime: null,
    duration: 0,
    limitPassengers: 0,
    status: 'Agendada',
    passengers: []
  };

  beforeEach(async () => {
    tripService = jasmine.createSpyObj<TripService>(
      'TripService',
      ['getTrips', 'getBuses', 'getDrivers', 'getRoutes', 'getMyTrips', 'createTrip', 'updateTrip', 'deleteTrip']
    );
    routeService = jasmine.createSpyObj<RouteService>(
      'RouteService',
      ['getRoutes', 'createRoute', 'updateRoute', 'deleteRoute']
    );
    driverService = jasmine.createSpyObj<DriverService>(
      'DriverService',
      ['getDrivers', 'createDriver', 'updateDriver', 'deleteDriver']
    );
    sessionService = jasmine.createSpyObj<SessionService>(
      'SessionService',
      ['isAdmin', 'isAuthenticated']
    );
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);

    tripService.getTrips.and.returnValue(of([trip]));
    tripService.getBuses.and.returnValue(of([trip.bus!]));
    driverService.getDrivers.and.returnValue(of([trip.driver!]));
    routeService.getRoutes.and.returnValue(of([trip.route!]));

    await TestBed.configureTestingModule({
      declarations: [TripsComponent],
      providers: [
        { provide: TripService, useValue: tripService },
        { provide: RouteService, useValue: routeService },
        { provide: DriverService, useValue: driverService },
        { provide: SessionService, useValue: sessionService },
        { provide: Router, useValue: router }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(TripsComponent);
    component = fixture.componentInstance;
  });

  it('creates and loads trips and related form options', () => {
    fixture.detectChanges();

    expect(component).toBeTruthy();
    expect(component.trips).toEqual([trip]);
    expect(component.buses.length).toBe(1);
    expect(component.drivers.length).toBe(1);
    expect(component.routes.length).toBe(1);
    expect(component.isLoading).toBeFalse();
  });

  it('shows an error when initial trip data cannot be loaded', () => {
    tripService.getTrips.and.returnValue(
      throwError(() => new Error('API indisponível'))
    );

    component.carregarDados();

    expect(component.errorMessage).toContain('Não foi possível carregar os dados das viagens');
    expect(component.isLoading).toBeFalse();
  });

  it('does not allow editing a completed trip', () => {
    component.iniciarEdicao({ ...trip, status: 'Concluída' });

    expect(component.editingTrip).toBeNull();
    expect(component.errorMessage).toContain('não podem ser alteradas');
  });

  it('does not allow editing a cancelled trip', () => {
    component.iniciarEdicao({ ...trip, status: 'Cancelada' });

    expect(component.editingTrip).toBeNull();
    expect(component.errorMessage).toContain('não podem ser alteradas');
  });

  it('prevents saving when required trip fields are missing', () => {
    component.iniciarCriacao();
    component.salvarViagem();

    expect(tripService.createTrip).not.toHaveBeenCalled();
    expect(component.errorMessage).toContain('Informe o ônibus');
  });

  it('does not allow completed trips to be changed or deleted', () => {
    const completed = { ...trip, status: 'Concluída' as const };

    component.excluirViagem(completed);

    expect(tripService.deleteTrip).not.toHaveBeenCalled();
    expect(component.errorMessage).toContain('não podem ser excluídas');
  });

  it('navigates back to the dashboard', () => {
    component.voltarAoDashboard();

    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('identifies related entities safely when information is missing', () => {
    const incomplete = { ...trip, bus: null, driver: null, route: null };

    expect(component.identificarOnibus(incomplete)).toBe('Não informado');
    expect(component.identificarMotorista(incomplete)).toBe('Não informado');
    expect(component.identificarRota(incomplete)).toBe('Não informada');
  });
});
