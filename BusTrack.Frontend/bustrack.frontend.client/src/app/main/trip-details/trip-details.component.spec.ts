import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { of, throwError } from 'rxjs';

import { TripDetailsComponent } from './trip-details.component';
import { TripService, Trip } from '../../services/trip.service';

describe('TripDetailsComponent', () => {
  let component: TripDetailsComponent;
  let fixture: ComponentFixture<TripDetailsComponent>;
  let tripService: jasmine.SpyObj<TripService>;
  let router: jasmine.SpyObj<Router>;
  let tripId: string | null;

  const trip: Trip = {
    id: 'trip-1',
    bus: {
      id: 'bus-1',
      number: '101',
      licensePlate: 'ABC1D23',
      model: 'Modelo',
      capacity: 40
    },
    driver: { id: 'driver-1', name: 'Motorista', licenseNumber: '123' },
    route: {
      id: 'route-1',
      name: 'Centro',
      description: '',
      origin: 'Origem',
      destination: 'Destino',
      distance: 10
    },
    departureTime: '2026-10-20T10:00:00Z',
    plannedDurationMinutes: 60,
    startedAt: '2026-10-20T10:00:00Z',
    arrivalTime: '2026-10-20T10:30:00Z',
    duration: 30,
    limitPassengers: 12,
    status: 'Concluída',
    passengers: []
  };

  beforeEach(async () => {
    tripId = 'trip-1';

    tripService = jasmine.createSpyObj<TripService>(
      'TripService',
      ['getTripById']
    );

    router = jasmine.createSpyObj<Router>('Router', ['navigate']);
    tripService.getTripById.and.returnValue(of(trip));

    await TestBed.configureTestingModule({
      declarations: [TripDetailsComponent],
      providers: [
        { provide: TripService, useValue: tripService },
        { provide: Router, useValue: router },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: () => tripId
              }
            }
          }
        }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(TripDetailsComponent);
    component = fixture.componentInstance;
  });

  it('loads the requested trip', () => {
    fixture.detectChanges();

    expect(tripService.getTripById).toHaveBeenCalledWith('trip-1');
    expect(component.trip).toEqual(trip);
    expect(component.isLoading).toBeFalse();
  });

  it('does not call the service when the route has no identifier', () => {
    tripId = null;
    fixture.detectChanges();

    expect(tripService.getTripById).not.toHaveBeenCalled();
    expect(component.notFound).toBeTrue();
    expect(component.errorMessage).toContain('identificador');
  });

  it('handles a trip that does not exist', () => {
    tripService.getTripById.and.returnValue(
      throwError(() => new HttpErrorResponse({ status: 404 }))
    );

    fixture.detectChanges();

    expect(component.notFound).toBeTrue();
    expect(component.errorMessage).toContain('não foi encontrada');
    expect(component.isLoading).toBeFalse();
  });

  it('shows a connection error when the API cannot be reached', () => {
    tripService.getTripById.and.returnValue(
      throwError(() => new HttpErrorResponse({ status: 0 }))
    );

    fixture.detectChanges();

    expect(component.errorMessage).toContain(
      'Não foi possível conectar à API'
    );
    expect(component.isLoading).toBeFalse();
  });

  it('formats related trip information safely when relations are absent', () => {
    component.trip = null;

    expect(component.identificarOnibus()).toBe('Não informado');
    expect(component.identificarMotorista()).toBe('Não informado');
    expect(component.identificarRota()).toBe('Não informada');
    expect(component.quantidadePassageiros()).toBe(0);
  });

  it('navigates back to the trip list', () => {
    component.voltarParaViagens();

    expect(router.navigate).toHaveBeenCalledWith(['/trips']);
  });
});
