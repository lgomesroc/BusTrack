import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';

export interface Passenger {
  id: string;
  name: string;
  email: string;
  cpf: string;
  phone: string;
}

export type PassengerPayload = Omit<Passenger, 'id'>;

@Injectable({
  providedIn: 'root'
})
export class PassengerService {
  private readonly apiUrl = `${environment.apiUrl}/api/Passenger`;

  constructor(private http: HttpClient) {}

  getPassengers(): Observable<Passenger[]> {
    return this.http.get<Passenger[]>(this.apiUrl);
  }

  createPassenger(
    passenger: PassengerPayload
  ): Observable<Passenger> {
    return this.http.post<Passenger>(this.apiUrl, passenger);
  }

  updatePassenger(
    id: string,
    passenger: PassengerPayload
  ): Observable<Passenger> {
    return this.http.put<Passenger>(
      `${this.apiUrl}/${id}`,
      passenger
    );
  }

  deletePassenger(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
