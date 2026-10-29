import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { environment } from '../../environments/environment';
import { OperationalReport, OperationalReportService } from './operational-report.service';

describe('OperationalReportService', () => {
  let service: OperationalReportService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [OperationalReportService]
    });

    service = TestBed.inject(OperationalReportService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('requests a report using the selected date range', () => {
    const report: OperationalReport = {
      startDate: '2026-10-01T00:00:00',
      endDate: '2026-10-27T00:00:00',
      totalTrips: 2,
      completedTrips: 1,
      cancelledTrips: 0,
      scheduledTrips: 1,
      inProgressTrips: 0,
      totalMaintenanceCost: 150,
      busUtilization: [],
      driverTrips: [],
      maintenanceCosts: []
    };

    service.getReport('2026-10-01', '2026-10-27').subscribe(response => {
      expect(response).toEqual(report);
    });

    const request = httpMock.expectOne(
      `${environment.apiUrl}/api/OperationalReports?startDate=2026-10-01&endDate=2026-10-27`
    );

    expect(request.request.method).toBe('GET');
    request.flush(report);
  });
});
