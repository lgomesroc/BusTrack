// user.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserData } from '../models/user.model'; // Importe a interface UserData

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = 'https://seuservidor.com/api/user'; // Substitua pela URL real do seu backend

  constructor(private http: HttpClient) { }

  getUserData(): Observable<UserData> {
    return this.http.get<UserData>(`${this.apiUrl}/data`);
  }
}
