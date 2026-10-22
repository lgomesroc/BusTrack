
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export type MaintenanceStatus =
  | 'Agendada'
  | 'Em andamento'
  | 'Concluída'
  | 'Cancelada';

export interface Maintenance {
  id: string;
  busId: string;
  busNumber: string;
  serviceType: string;
  description: string;
  serviceDate: string;
  completedDate?: string | null;
  cost: number;
  status: MaintenanceStatus;
}

export type MaintenancePayload = Omit<Maintenance, 'id' | 'busNumber'>;

@Injectable({
  providedIn: 'root'
})
export class MaintenanceService {
  private readonly apiUrl = `${environment.apiUrl}/api/Maintenance`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Maintenance[]> {
    return this.http.get<Maintenance[]>(this.apiUrl);
  }

  create(payload: MaintenancePayload): Observable<Maintenance> {
    return this.http.post<Maintenance>(this.apiUrl, payload);
  }

  update(
    id: string,
    payload: MaintenancePayload
  ): Observable<Maintenance> {
    return this.http.put<Maintenance>(`${this.apiUrl}/${id}`, payload);
  }

  delete(id: string, reason: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, {
      params: { reason }
    });
  }
}
