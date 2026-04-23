import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Property } from '../models/booking.model';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class PropertyService {
  private readonly apiUrl = `${environment.apiUrl}/property`;
  private readonly http = inject(HttpClient);

  getAll(): Observable<Property[]> {
    return this.http.get<Property[]>(this.apiUrl);
  }
}