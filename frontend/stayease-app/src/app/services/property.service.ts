import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Property } from '../models/booking.model';

@Injectable({ providedIn: 'root' })
export class PropertyService {
  private readonly apiUrl = 'http://localhost:5141/api/property';
  private readonly http = inject(HttpClient);

  getAll(): Observable<Property[]> {
    return this.http.get<Property[]>(this.apiUrl);
  }
}
