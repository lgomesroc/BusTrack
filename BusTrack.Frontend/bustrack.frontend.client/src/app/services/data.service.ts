import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class DataService {
    constructor(private http: HttpClient) { }

    saveDataToMongoDB(data: any): Observable<any> {
      return this.http.post('http://localhost:7072', data);
    }
}
