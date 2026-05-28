import { Component, inject, signal, ElementRef, ViewChild, OnInit } from '@angular/core';
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
import { PropertyService } from '../../services/property.service';
import { BookingResponse } from '../../models/booking.model';
import { Property, PropertySearchFilters } from '../../models/property.model';
import { BookingPropertySearchComponent } from '../../components/booking/booking-property-search';

function dateRangeValidator(control: AbstractControl): ValidationErrors | null {
  const start = control.get('startDate')?.value;
  const end = control.get('endDate')?.value;
  if (start && end && new Date(end) <= new Date(start)) {
    return { dateRange: true };
  }
  return null;
}

@Component({
  selector: 'app-booking',
  imports: [ReactiveFormsModule, DecimalPipe, BookingPropertySearchComponent],
  templateUrl: './booking.html',
})
export class BookingComponent implements OnInit {
  @ViewChild('formSection') formSection!: ElementRef;

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private bookingService = inject(BookingService);
  private propertyService = inject(PropertyService);

  properties = signal<Property[]>([]);
  filteredProperties = signal<Property[]>([]);
  propertiesError = signal<string | null>(null);
  isSearching = signal(false);
  selectedProperty = signal<Property | null>(null);
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

  ngOnInit(): void {
    this.propertyService.getAll().subscribe({
      next: (data) => {
        this.properties.set(data);
        this.filteredProperties.set(data);
      },
      error: () => this.propertiesError.set('Failed to load properties. Please try again later.'),
    });
  }

  onFiltersApplied(filters: PropertySearchFilters): void {
    const hasFilters = !!(filters.location || filters.minGuests || filters.maxGuests || filters.minPrice || filters.maxPrice);

    if (!hasFilters) {
      this.filteredProperties.set(this.properties());
      this.propertiesError.set(null);
      return;
    }

    this.isSearching.set(true);
    this.propertiesError.set(null);

    this.propertyService.search(filters).subscribe({
      next: (data) => {
        this.filteredProperties.set(data);
        this.isSearching.set(false);
      },
      error: () => {
        this.propertiesError.set('Failed to search properties. Please try again.');
        this.isSearching.set(false);
      },
    });
  }

  selectProperty(property: Property): void {
    this.selectedProperty.set(property);
    this.bookingResult.set(null);
    this.bookingError.set(null);
    this.bookingForm.reset();
    setTimeout(() => {
      this.formSection?.nativeElement?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }, 50);
  }

  onSubmit(): void {
    this.bookingForm.markAllAsTouched();
    if (this.bookingForm.invalid || !this.selectedProperty()) return;

    const { startDate, endDate } = this.bookingForm.value;
    const userID = this.authService.currentUser()?.userID;
    const property = this.selectedProperty()!;

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
            typeof err.error === 'string' ? err.error : 'Something went wrong. Please try again.'
          );
          this.bookingForm.reset();
          this.isLoading.set(false);
        },
      });
  }

  closeBookingPanel(): void {
    this.selectedProperty.set(null);
    this.bookingResult.set(null);
    this.bookingError.set(null);
    this.bookingForm.reset();
  }

  get nights(): number {
    const { startDate, endDate } = this.bookingForm.value;
    if (!startDate || !endDate) return 0;
    const diff = new Date(endDate).getTime() - new Date(startDate).getTime();
    return Math.max(0, Math.floor(diff / (1000 * 60 * 60 * 24)));
  }

  get estimatedTotal(): number {
    const property = this.selectedProperty();
    if (!property) return 0;
    return this.nights * property.pricePerNight;
  }
}
