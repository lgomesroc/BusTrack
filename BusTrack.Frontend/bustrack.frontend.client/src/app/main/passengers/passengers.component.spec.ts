import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { PassengersComponent } from './passengers.component';
import { PassengerService, Passenger } from '../../services/passenger.service';
import { SessionService } from '../../services/session.service';

describe('PassengersComponent', () => {
  let component: PassengersComponent;
  let fixture: ComponentFixture<PassengersComponent>;
  let passengerService: jasmine.SpyObj<PassengerService>;
  let sessionService: jasmine.SpyObj<SessionService>;
  let router: jasmine.SpyObj<Router>;

  const passenger: Passenger = {
    id: 'passenger-1',
    name: 'Passageiro Teste',
    email: 'passageiro@example.com',
    cpf: '12345678901',
    phone: '21999999999'
  };

  beforeEach(async () => {
    passengerService = jasmine.createSpyObj<PassengerService>(
      'PassengerService',
      ['getPassengers', 'createPassenger', 'updatePassenger', 'deletePassenger']
    );
    sessionService = jasmine.createSpyObj<SessionService>(
      'SessionService',
      ['isAdmin', 'isAuthenticated']
    );
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);
    passengerService.getPassengers.and.returnValue(of([passenger]));

    await TestBed.configureTestingModule({
      declarations: [PassengersComponent],
      providers: [
        { provide: PassengerService, useValue: passengerService },
        { provide: SessionService, useValue: sessionService },
        { provide: Router, useValue: router }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(PassengersComponent);
    component = fixture.componentInstance;
  });

  it('creates and loads passengers on initialization', () => {
    fixture.detectChanges();

    expect(component).toBeTruthy();
    expect(component.passengers).toEqual([passenger]);
    expect(component.isLoading).toBeFalse();
  });

  it('formats CPF digits', () => {
    expect(component.formatCpf('12345678901')).toBe('123.456.789-01');
    expect(component.formatCpf(undefined)).toBe('');
  });

  it('does not save a passenger without required fields', () => {
    component.startCreate();
    component.savePassenger();

    expect(passengerService.createPassenger).not.toHaveBeenCalled();
    expect(component.errorMessage).toContain('Preencha o nome e o CPF');
  });

  it('creates a passenger with valid data', () => {
    passengerService.createPassenger.and.returnValue(of(passenger));
    component.startCreate();
    component.passengerForm = {
      name: passenger.name,
      email: passenger.email,
      cpf: passenger.cpf,
      phone: passenger.phone
    };

    component.savePassenger();

    expect(passengerService.createPassenger).toHaveBeenCalledWith({
      name: passenger.name,
      email: passenger.email,
      cpf: passenger.cpf,
      phone: passenger.phone
    });
    expect(component.passengers).toContain(passenger);
    expect(component.isLoading).toBeFalse();
  });

  it('shows an error when the passenger list fails to load', () => {
    passengerService.getPassengers.and.returnValue(
      throwError(() => new Error('API indisponível'))
    );

    component.loadPassengers();

    expect(component.errorMessage).toContain('Não foi possível carregar os passageiros');
    expect(component.isLoading).toBeFalse();
  });

  it('navigates back to the dashboard', () => {
    component.backToDashboard();

    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });
});
