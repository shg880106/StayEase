import { Component, inject, signal, ElementRef, ViewChild, OnInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { BookingService } from '../../services/booking.service';
import { PropertyService } from '../../services/property.service';
import { Property, BookingResponse } from '../../models/booking.model';

const GUID_PATTERN =
  /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

function guidValidator(control: AbstractControl): ValidationErrors | null {
  if (!control.value) return null;
  return GUID_PATTERN.test(control.value) ? null : { invalidGuid: true };
}

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
  imports: [ReactiveFormsModule, DecimalPipe],
  templateUrl: './booking.html',
})
export class BookingComponent implements OnInit {
  @ViewChild('formSection') formSection!: ElementRef;

  private fb = inject(FormBuilder);
  private bookingService = inject(BookingService);
  private propertyService = inject(PropertyService);

  properties = signal<Property[]>([]);
  propertiesError = signal<string | null>(null);
  selectedProperty = signal<Property | null>(null);
  bookingResult = signal<BookingResponse | null>(null);
  bookingError = signal<string | null>(null);
  isLoading = signal(false);

  today = new Date().toISOString().split('T')[0];

  bookingForm = this.fb.group(
    {
      userID: ['', [Validators.required, guidValidator]],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
    },
    { validators: dateRangeValidator }
  );

  ngOnInit(): void {
    this.propertyService.getAll().subscribe({
      next: (data) => this.properties.set(data),
      error: () => this.propertiesError.set('Failed to load properties. Please try again later.'),
    });
  }

  get userIDErrors() {
    const ctrl = this.bookingForm.get('userID');
    if (!ctrl?.touched) return null;
    if (ctrl.errors?.['required']) return 'User ID is required.';
    if (ctrl.errors?.['invalidGuid']) return 'Must be a valid UUID (e.g. 3fa85f64-5717-4562-b3fc-2c963f66afa6).';
    return null;
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

    const { userID, startDate, endDate } = this.bookingForm.value;
    const property = this.selectedProperty()!;

    this.isLoading.set(true);
    this.bookingError.set(null);
    this.bookingResult.set(null);

    this.bookingService
      .createBooking({
        propertyID: property.propertyID,
        userID: userID!,
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
          this.isLoading.set(false);
        },
      });
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
