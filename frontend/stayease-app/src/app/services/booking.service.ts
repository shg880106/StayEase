import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateBookingRequest, BookingResponse } from '../models/booking.model';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private readonly apiUrl = `${environment.apiUrl}/booking`;
  private readonly http = inject(HttpClient);

  createBooking(request: CreateBookingRequest): Observable<BookingResponse> {
    return this.http.post<BookingResponse>(this.apiUrl, request);
  }
}