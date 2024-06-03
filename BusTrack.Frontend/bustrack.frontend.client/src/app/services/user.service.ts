import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserData } from '../models/user.model'; 

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = 'http://localhost:7072/api/user'; 

  constructor(private http: HttpClient) { }

  getUserData(): Observable<UserData> {
    return this.http.get<UserData>(`${this.apiUrl}/data`);
  }
}
