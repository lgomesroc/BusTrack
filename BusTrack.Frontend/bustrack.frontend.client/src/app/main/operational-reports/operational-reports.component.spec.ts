import { CommonModule } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';

import { OperationalReportsComponent } from './operational-reports.component';
import { OperationalReport, OperationalReportService } from '../../services/operational-report.service';
import { SessionService } from '../../services/session.service';

describe('OperationalReportsComponent', () => {
  let component: OperationalReportsComponent;
  let fixture: ComponentFixture<OperationalReportsComponent>;
  let router: Router;
  let isAdmin: boolean;
  let reportService: jasmine.SpyObj<OperationalReportService>;
  let sessionService: jasmine.SpyObj<SessionService>;

  const report: OperationalReport = {
    startDate: '2026-10-01T00:00:00',
    endDate: '2026-10-27T00:00:00',
    totalTrips: 3,
    completedTrips: 1,
    cancelledTrips: 1,
    scheduledTrips: 1,
    inProgressTrips: 0,
    totalMaintenanceCost: 250.5,
    busUtilization: [
      { busId: 'bus-1', busNumber: '101', tripCount: 3, completedTripCount: 1 }
    ],
    driverTrips: [
      { driverId: 'driver-1', driverName: 'Motorista Teste', tripCount: 3, completedTripCount: 1 }
    ],
    maintenanceCosts: [
      { busId: 'bus-1', busNumber: '101', totalCost: 250.5, maintenanceCount: 1 }
    ]
  };

  function initializeComponent(): void {
    fixture = TestBed.createComponent(OperationalReportsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  beforeEach(async () => {
    isAdmin = true;
    reportService = jasmine.createSpyObj<OperationalReportService>(
      'OperationalReportService',
      ['getReport']
    );
    reportService.getReport.and.returnValue(of(report));

    sessionService = jasmine.createSpyObj<SessionService>(
      'SessionService',
      ['isAdmin']
    );
    sessionService.isAdmin.and.callFake(() => isAdmin);

    await TestBed.configureTestingModule({
      declarations: [OperationalReportsComponent],
      imports: [CommonModule, FormsModule, HttpClientTestingModule, RouterTestingModule],
      providers: [
        { provide: OperationalReportService, useValue: reportService },
        { provide: SessionService, useValue: sessionService }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    router = TestBed.inject(Router);
    spyOn(router, 'navigate').and.returnValue(Promise.resolve(true));
  });

  afterEach(() => {
    fixture?.destroy();
    TestBed.resetTestingModule();
  });

  it('loads and displays the operational report for an administrator', () => {
    initializeComponent();

    expect(reportService.getReport).toHaveBeenCalledWith(component.startDate, component.endDate);
    expect(component.report).toEqual(report);
    expect(component.isLoading).toBeFalse();
    expect(fixture.nativeElement.textContent).toContain('Relatórios operacionais');
    expect(fixture.nativeElement.textContent).toContain('Motorista Teste');
    expect(fixture.nativeElement.textContent).toContain('R$');
  });

  it('rejects an initial date after the final date without calling the API', () => {
    initializeComponent();
    reportService.getReport.calls.reset();
    component.startDate = '2026-10-28';
    component.endDate = '2026-10-01';

    component.loadReport();

    expect(component.errorMessage).toBe('A data inicial não pode ser posterior à data final.');
    expect(reportService.getReport).not.toHaveBeenCalled();
  });

  it('shows an empty-period message when there are no trips or maintenance costs', () => {
    reportService.getReport.and.returnValue(of({
      ...report,
      totalTrips: 0,
      completedTrips: 0,
      cancelledTrips: 0,
      scheduledTrips: 0,
      inProgressTrips: 0,
      totalMaintenanceCost: 0,
      busUtilization: [],
      driverTrips: [],
      maintenanceCosts: []
    }));

    initializeComponent();

    expect(fixture.nativeElement.textContent).toContain(
      'Não há viagens nem custos de manutenção para o período selecionado.'
    );
    expect(fixture.nativeElement.textContent).toContain(
      'Nenhuma viagem associada a ônibus foi encontrada neste período.'
    );
  });

  it('shows an API error message when the request fails', () => {
    reportService.getReport.and.returnValue(
      throwError(() => ({ error: { message: 'Período inválido.' } }))
    );

    initializeComponent();

    expect(component.errorMessage).toBe('Período inválido.');
    expect(component.isLoading).toBeFalse();
  });

  it('redirects non-administrators back to the dashboard without requesting reports', () => {
    isAdmin = false;

    initializeComponent();

    expect(router.navigate).toHaveBeenCalledWith(
      ['/dashboard'],
      { replaceUrl: true }
    );
    expect(reportService.getReport).not.toHaveBeenCalled();
  });
});
