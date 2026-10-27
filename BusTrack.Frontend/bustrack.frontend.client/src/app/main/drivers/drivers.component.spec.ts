import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { DriversComponent } from './drivers.component';
import { DriverService, Driver } from '../../services/driver.service';
import { SessionService } from '../../services/session.service';

describe('DriversComponent', () => {
  let component: DriversComponent;
  let fixture: ComponentFixture<DriversComponent>;
  let driverService: jasmine.SpyObj<DriverService>;
  let sessionService: jasmine.SpyObj<SessionService>;
  let router: jasmine.SpyObj<Router>;

  const driver: Driver = {
    id: 'driver-1',
    name: 'Motorista Teste',
    licenseNumber: '12345678901'
  };

  beforeEach(async () => {
    driverService = jasmine.createSpyObj<DriverService>(
      'DriverService',
      ['getDrivers', 'createDriver', 'updateDriver', 'deleteDriver']
    );
    sessionService = jasmine.createSpyObj<SessionService>(
      'SessionService',
      ['isAdmin', 'isAuthenticated']
    );
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);
    driverService.getDrivers.and.returnValue(of([driver]));

    await TestBed.configureTestingModule({
      declarations: [DriversComponent],
      providers: [
        { provide: DriverService, useValue: driverService },
        { provide: SessionService, useValue: sessionService },
        { provide: Router, useValue: router }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(DriversComponent);
    component = fixture.componentInstance;
  });

  it('creates and loads drivers on initialization', () => {
    fixture.detectChanges();

    expect(component).toBeTruthy();
    expect(component.drivers).toEqual([driver]);
    expect(component.isLoading).toBeFalse();
  });

  it('formats CPF digits', () => {
    expect(component.formatCpf('12345678901')).toBe('123.456.789-01');
    expect(component.formatCpf('abc')).toBe('');
  });

  it('does not save a driver without a name and CPF', () => {
    component.startCreate();
    component.saveDriver();

    expect(driverService.createDriver).not.toHaveBeenCalled();
    expect(component.errorMessage).toContain('Preencha o nome');
  });

  it('creates a driver with valid data', () => {
    driverService.createDriver.and.returnValue(of(driver));
    component.startCreate();
    component.driverForm = { ...driver, id: '', licenseNumber: '12345678901' };

    component.saveDriver();

    expect(driverService.createDriver).toHaveBeenCalledWith({
      name: driver.name,
      licenseNumber: '12345678901'
    });
    expect(component.drivers).toContain(driver);
    expect(component.isLoading).toBeFalse();
  });

  it('shows an error when the driver list fails to load', () => {
    driverService.getDrivers.and.returnValue(
      throwError(() => new Error('API indisponível'))
    );

    component.loadDrivers();

    expect(component.errorMessage).toContain('Não foi possível carregar os motoristas');
    expect(component.isLoading).toBeFalse();
  });

  it('navigates back to the dashboard', () => {
    component.backToDashboard();

    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });
});
