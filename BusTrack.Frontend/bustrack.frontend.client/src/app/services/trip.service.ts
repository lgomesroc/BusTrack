import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export type TripStatus = 'Agendada' | 'Em andamento' | 'Concluída' | 'Cancelada';

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
  startedAt: string | null;
  arrivalTime: string | null;
  duration: number;
  plannedDuration: number;
  limitPassengers: number;
  status: TripStatus;
  passengers: Passenger[];
}

export interface TripPayload {
  busId: string;
  driverId: string;
  routeId: string;
  departureTime: string;
  startedAt?: string | null;
  arrivalTime: string | null;
  duration: number;
  plannedDuration: number;
  limitPassengers: number;
  status: TripStatus;
  passengers: string[];
}

@Injectable({ providedIn: 'root' })
export class TripService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getTrips(): Observable<Trip[]> {
    return this.http.get<Trip[]>(`${this.apiUrl}/api/Trip`);
  }

  getTripById(id: string): Observable<Trip> {
    return this.http.get<Trip>(`${this.apiUrl}/api/Trip/${encodeURIComponent(id)}`);
  }

  getMyTrips(): Observable<Trip[]> {
    return this.http.get<Trip[]>(`${this.apiUrl}/api/Trip/mine`);
  }

  createTrip(trip: TripPayload): Observable<TripPayload> {
    return this.http.post<TripPayload>(`${this.apiUrl}/api/Trip`, trip);
  }

  updateTrip(id: string, trip: TripPayload): Observable<TripPayload> {
    return this.http.put<TripPayload>(`${this.apiUrl}/api/Trip/${encodeURIComponent(id)}`, trip);
  }

  deleteTrip(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/Trip/${encodeURIComponent(id)}`);
  }

  getBuses(): Observable<Bus[]> {
    return this.http.get<Bus[]>(`${this.apiUrl}/api/Bus`);
  }

  getDrivers(): Observable<Driver[]> {
    return this.http.get<Driver[]>(`${this.apiUrl}/api/Driver`);
  }

  getRoutes(): Observable<Route[]> {
    return this.http.get<Route[]>(`${this.apiUrl}/api/Route`);
  }
}