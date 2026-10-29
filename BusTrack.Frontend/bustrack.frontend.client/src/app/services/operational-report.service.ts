import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface BusUtilizationReport {
  busId: string;
  busNumber: string;
  tripCount: number;
  completedTripCount: number;
}

export interface DriverTripReport {
  driverId: string;
  driverName: string;
  tripCount: number;
  completedTripCount: number;
}

export interface MaintenanceCostReport {
  busId: string;
  busNumber: string;
  totalCost: number;
  maintenanceCount: number;
}

export interface OperationalReport {
  startDate: string;
  endDate: string;
  totalTrips: number;
  completedTrips: number;
  cancelledTrips: number;
  scheduledTrips: number;
  inProgressTrips: number;
  totalMaintenanceCost: number;
  busUtilization: BusUtilizationReport[];
  driverTrips: DriverTripReport[];
  maintenanceCosts: MaintenanceCostReport[];
}

@Injectable({ providedIn: 'root' })
export class OperationalReportService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private readonly http: HttpClient) {}

  getReport(startDate: string, endDate: string): Observable<OperationalReport> {
    const params = new HttpParams()
      .set('startDate', startDate)
      .set('endDate', endDate);

    return this.http.get<OperationalReport>(
      `${this.apiUrl}/api/OperationalReports`,
      { params }
    );
  }
}
