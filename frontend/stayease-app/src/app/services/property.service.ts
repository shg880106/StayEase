import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Property, PropertySearchFilters } from '../models/property.model';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class PropertyService {
  private readonly apiUrl = `${environment.apiUrl}/property`;
  private readonly http = inject(HttpClient);

  getAll(): Observable<Property[]> {
    return this.http.get<Property[]>(this.apiUrl);
  }

  search(filters: PropertySearchFilters): Observable<Property[]> {
    let params = new HttpParams();
    if (filters.location) params = params.set('location', filters.location);
    if (filters.minPrice != null) params = params.set('minPrice', filters.minPrice.toString());
    if (filters.maxPrice != null) params = params.set('maxPrice', filters.maxPrice.toString());
    if (filters.minGuests != null) params = params.set('minGuests', filters.minGuests.toString());
    if (filters.maxGuests != null) params = params.set('maxGuests', filters.maxGuests.toString());
    return this.http.get<Property[]>(`${this.apiUrl}/search/filter`, { params });
  }
}