import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Bus {
  id: string;
  number: string;
  licensePlate: string;
  model: string;
  capacity: number;
}

@Injectable({
  providedIn: 'root'
})
export class BusService {

  private readonly apiUrl =
    'http://localhost:5066/api/BusControllerAPI';

  constructor(private http: HttpClient) {}

  getBuses(): Observable<Bus[]> {
    return this.http.get<Bus[]>(this.apiUrl);
  }

  createBus(
    bus: Omit<Bus, 'id'>
  ): Observable<Bus> {
    return this.http.post<Bus>(
      this.apiUrl,
      bus
    );
  }

  updateBus(
    id: string,
    bus: Omit<Bus, 'id'>
  ): Observable<Bus> {
    return this.http.put<Bus>(
      `${this.apiUrl}/${id}`,
      bus
    );
  }

  deleteBus(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}
