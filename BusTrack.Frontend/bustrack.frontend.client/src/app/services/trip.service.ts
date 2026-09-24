import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';

export interface Bus {
  id: string;
  number: string;
  licensePlate: string;
  model: string;
  capacity: number;
}

export interface Driver {
  id: string;
  name: string;
  licenseNumber: string;
}

export interface Route {
  id: string;
  name: string;
  description: string;
  origin: string;
  destination: string;
  distance: number;
}

export interface Passenger {
  id: string;
  name: string;
  cpf: string;
  email: string;
  phone: string;
}

export interface Trip {
  id: string;

  bus: Bus | null;

  driver: Driver | null;

  route: Route | null;

  departureTime: string;

  arrivalTime: string;

  duration: number;

  limitPassengers: number;

  passengers: Passenger[];
}

export interface TripPayload {
  busId: string;
  driverId: string;
  routeId: string;
  departureTime: string;
  arrivalTime: string;
  duration: number;
  limitPassengers: number;
  passengers: string[];
}

@Injectable({
  providedIn: 'root'
})
export class TripService {

  private readonly apiUrl =
    environment.apiUrl;

  constructor(
    private http: HttpClient
  ) {}

  getTrips(): Observable<Trip[]> {
    return this.http.get<Trip[]>(
      `${this.apiUrl}/api/TripControllerAPI`
    );
  }

  createTrip(
    trip: TripPayload
  ): Observable<TripPayload> {
    return this.http.post<TripPayload>(
      `${this.apiUrl}/api/TripControllerAPI`,
      trip
    );
  }

  updateTrip(
    id: string,
    trip: TripPayload
  ): Observable<TripPayload> {
    return this.http.put<TripPayload>(
      `${this.apiUrl}/api/TripControllerAPI/${id}`,
      trip
    );
  }

  deleteTrip(
    id: string
  ): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/api/TripControllerAPI/${id}`
    );
  }

  getBuses(): Observable<Bus[]> {
    return this.http.get<Bus[]>(
      `${this.apiUrl}/api/BusControllerAPI`
    );
  }

  getDrivers(): Observable<Driver[]> {
    return this.http.get<Driver[]>(
      `${this.apiUrl}/api/DriverControllerAPI`
    );
  }

  getRoutes(): Observable<Route[]> {
    return this.http.get<Route[]>(
      `${this.apiUrl}/api/RouteControllerAPI`
    );
  }
}
