import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Trip {
  id: string;
  busId: string;
  driverId: string;
  routeId: string;
  departureTime: string;
  arrivalTime: string;
  duration: number;
  limitPassengers: number;
  passengers: string[];
}

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

@Injectable({
  providedIn: 'root'
})
export class TripService {

  private readonly apiUrl =
    'http://localhost:5066';

  constructor(
    private http: HttpClient
  ) {}

  getTrips(): Observable<Trip[]> {
    return this.http.get<Trip[]>(
      `${this.apiUrl}/api/TripControllerAPI`
    );
  }

  createTrip(
    trip: Omit<Trip, 'id'>
  ): Observable<Trip> {
    return this.http.post<Trip>(
      `${this.apiUrl}/api/TripControllerAPI`,
      trip
    );
  }

  updateTrip(
    id: string,
    trip: Omit<Trip, 'id'>
  ): Observable<Trip> {
    return this.http.put<Trip>(
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
