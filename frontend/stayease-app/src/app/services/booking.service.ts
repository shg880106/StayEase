import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateBookingRequest, BookingResponse, MyBooking, BookingDetails, BookingDetailsForOwnerDto } from '../models/booking.model';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private readonly apiUrl = `${environment.apiUrl}/booking`;
  private readonly http = inject(HttpClient);

  createBooking(request: CreateBookingRequest): Observable<BookingResponse> {
    return this.http.post<BookingResponse>(this.apiUrl, request);
  }

  getMyBookings(): Observable<MyBooking[]> {
    return this.http.get<MyBooking[]>(`${this.apiUrl}/my-bookings`);
  }

  getBookingDetails(bookingID: string): Observable<BookingDetails> {
    return this.http.get<BookingDetails>(`${this.apiUrl}/${bookingID}`);
  }

  cancelBooking(bookingID: string): Observable<BookingDetails> {
    return this.http.request<BookingDetails>('PATCH', `${this.apiUrl}/${bookingID}/cancel`, {});
  }

  confirmBooking(bookingID: string): Observable<BookingResponse> {
    return this.http.request<BookingResponse>('PATCH', `${this.apiUrl}/${bookingID}/confirm`, {});
  }

  finishBooking(bookingID: string): Observable<BookingResponse> {
    return this.http.request<BookingResponse>('PATCH', `${this.apiUrl}/${bookingID}/finish`, {});
  }

  getPropertyBookings(propertyID: string): Observable<MyBooking[]> {
    return this.http.get<MyBooking[]>(`${this.apiUrl}/property/${propertyID}`);
  }

  getBookingDetailsForOwner(bookingID: string): Observable<BookingDetailsForOwnerDto> {
    return this.http.get<BookingDetailsForOwnerDto>(`${this.apiUrl}/my-properties/${bookingID}`);
  }
}
