import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class DataService {
    constructor(private http: HttpClient) { }

    saveDataToMongoDB(data: any): Observable<any> {
        // Substitua 'your-api-url' pela URL da sua API
        return this.http.post('your-api-url', data);
    }
}
