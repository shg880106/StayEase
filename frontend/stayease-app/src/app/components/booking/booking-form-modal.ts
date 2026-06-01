import { Component, inject, signal, input, output } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';

import { AuthService } from '../../services/auth.service';
import { BookingService } from '../../services/booking.service';
import { BookingResponse } from '../../models/booking.model';
import { Property } from '../../models/property.model';

function dateRangeValidator(control: AbstractControl): ValidationErrors | null {
  const start = control.get('startDate')?.value;
  const end = control.get('endDate')?.value;
  if (start && end && new Date(end) <= new Date(start)) {
    return { dateRange: true };
  }
  return null;
}

@Component({
  selector: 'app-booking-form-modal',
  imports: [ReactiveFormsModule, DecimalPipe],
  templateUrl: './booking-form-modal.html',
})
export class BookingFormModalComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private bookingService = inject(BookingService);

  readonly property = input.required<Property>();
  readonly closed = output<void>();

  bookingResult = signal<BookingResponse | null>(null);
  bookingError = signal<string | null>(null);
  isLoading = signal(false);

  today = new Date().toISOString().split('T')[0];

  bookingForm = this.fb.group(
    {
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
    },
    { validators: dateRangeValidator }
  );

  get nights(): number {
    const { startDate, endDate } = this.bookingForm.value;
    if (!startDate || !endDate) return 0;
    const diff = new Date(endDate).getTime() - new Date(startDate).getTime();
    return Math.max(0, Math.floor(diff / (1000 * 60 * 60 * 24)));
  }

  get estimatedTotal(): number {
    return this.nights * this.property().pricePerNight;
  }

  close(): void {
    this.closed.emit();
  }

  onSubmit(): void {
    this.bookingForm.markAllAsTouched();
    if (this.bookingForm.invalid) return;

    const { startDate, endDate } = this.bookingForm.value;
    const userID = this.authService.currentUser()?.userID;
    const property = this.property();

    if (!userID) {
      this.bookingError.set('You must be logged in to make a booking.');
      return;
    }

    this.isLoading.set(true);
    this.bookingError.set(null);
    this.bookingResult.set(null);

    this.bookingService
      .createBooking({
        propertyID: property.propertyID,
        userID,
        startDate: new Date(startDate!).toISOString(),
        endDate: new Date(endDate!).toISOString(),
      })
      .subscribe({
        next: (response) => {
          this.bookingResult.set(response);
          this.isLoading.set(false);
          this.bookingForm.reset();
        },
        error: (err) => {
          this.bookingError.set(
            typeof err.error === 'string'
              ? err.error
              : err.error?.message ?? 'Something went wrong. Please try again.'
          );
          this.bookingForm.reset();
          this.isLoading.set(false);
        },
      });
  }
}
