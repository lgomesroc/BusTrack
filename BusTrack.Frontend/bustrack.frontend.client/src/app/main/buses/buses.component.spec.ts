import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { BusesComponent } from './buses.component';
import { BusService, Bus } from '../../services/bus.service';
import { SessionService } from '../../services/session.service';

describe('BusesComponent', () => {
  let component: BusesComponent;
  let fixture: ComponentFixture<BusesComponent>;
  let busService: jasmine.SpyObj<BusService>;
  let sessionService: jasmine.SpyObj<SessionService>;
  let router: jasmine.SpyObj<Router>;

  const bus: Bus = {
    id: 'bus-1',
    number: '101',
    licensePlate: 'ABC1D23',
    model: 'Modelo teste',
    capacity: 40
  };

  beforeEach(async () => {
    busService = jasmine.createSpyObj<BusService>(
      'BusService',
      ['getBuses', 'createBus', 'updateBus', 'deleteBus']
    );
    sessionService = jasmine.createSpyObj<SessionService>(
      'SessionService',
      ['isAdmin', 'isAuthenticated']
    );
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);
    busService.getBuses.and.returnValue(of([bus]));

    await TestBed.configureTestingModule({
      declarations: [BusesComponent],
      providers: [
        { provide: BusService, useValue: busService },
        { provide: SessionService, useValue: sessionService },
        { provide: Router, useValue: router }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(BusesComponent);
    component = fixture.componentInstance;
  });

  it('creates and loads buses on initialization', () => {
    fixture.detectChanges();

    expect(component).toBeTruthy();
    expect(busService.getBuses).toHaveBeenCalled();
    expect(component.buses).toEqual([bus]);
    expect(component.isLoading).toBeFalse();
  });

  it('shows an error when buses cannot be loaded', () => {
    busService.getBuses.and.returnValue(
      throwError(() => new Error('API indisponível'))
    );

    component.carregarOnibus();

    expect(component.errorMessage).toContain('Não foi possível carregar os ônibus');
    expect(component.isLoading).toBeFalse();
  });

  it('does not create a bus when required fields are missing', () => {
    component.iniciarCriacao();
    component.salvarOnibus();

    expect(busService.createBus).not.toHaveBeenCalled();
    expect(component.errorMessage).toContain('Preencha o número');
  });

  it('creates a bus with valid data', () => {
    busService.createBus.and.returnValue(of(bus));
    component.iniciarCriacao();
    component.busForm = { ...bus, id: '' };

    component.salvarOnibus();

    expect(busService.createBus).toHaveBeenCalledWith({
      number: bus.number,
      licensePlate: bus.licensePlate,
      model: bus.model,
      capacity: bus.capacity
    });
    expect(component.buses).toContain(bus);
    expect(component.isLoading).toBeFalse();
  });

  it('navigates back to the dashboard', () => {
    component.voltarAoDashboard();

    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });
});
