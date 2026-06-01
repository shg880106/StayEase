import { Component, inject, OnInit, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgClass } from '@angular/common';
import { Button } from 'primeng/button';
import { PropertyService } from '../../services/property.service';
import { AuthService } from '../../services/auth.service';
import { BookingService } from '../../services/booking.service';
import { Property } from '../../models/property.model';
import { MyBooking, BookingDetailsForOwnerDto } from '../../models/booking.model';
import { BookingDetailsModalComponent, BookingDetailsModalData } from '../../components/booking/booking-details-modal';

const STATUS_LABELS: Record<number, string> = {
  1: 'Pending',
  2: 'Confirmed',
  3: 'Cancelled',
  4: 'Finished',
};

const STATUS_CLASSES: Record<number, string> = {
  1: 'bg-yellow-100 text-yellow-700',
  2: 'bg-green-100 text-green-700',
  3: 'bg-red-100 text-red-600',
  4: 'bg-blue-100 text-blue-700',
};

@Component({
  selector: 'app-my-properties',
  imports: [ReactiveFormsModule, NgClass, BookingDetailsModalComponent, Button, DecimalPipe],
  templateUrl: './my-properties.html',
})
export class MyPropertiesComponent implements OnInit {
  private readonly propertyService = inject(PropertyService);
  private readonly bookingService = inject(BookingService);
  readonly authService = inject(AuthService);
  private readonly fb = inject(FormBuilder);

  readonly properties = signal<Property[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly showModal = signal(false);
  readonly submitting = signal(false);
  readonly submitError = signal<string | null>(null);

  readonly showEditModal = signal(false);
  readonly editingProperty = signal<Property | null>(null);
  readonly editSubmitting = signal(false);
  readonly editSubmitError = signal<string | null>(null);
  readonly deletingId = signal<string | null>(null);

  readonly showDeleteConfirm = signal(false);
  readonly pendingDeleteId = signal<string | null>(null);
  readonly deleteSuccessTitle = signal<string | null>(null);

  readonly showBookingsModal = signal(false);
  readonly selectedPropertyForBookings = signal<Property | null>(null);
  readonly propertyBookings = signal<MyBooking[]>([]);
  readonly bookingsLoading = signal(false);
  readonly bookingsError = signal<string | null>(null);
  readonly confirmingId = signal<string | null>(null);
  readonly confirmSuccessId = signal<string | null>(null);
  readonly confirmErrorMessage = signal<string | null>(null);
  readonly finishingId = signal<string | null>(null);
  readonly finishSuccessId = signal<string | null>(null);
  readonly finishErrorMessage = signal<string | null>(null);
  readonly cancelSuccessId = signal<string | null>(null);
  readonly cancelErrorMessage = signal<string | null>(null);
  readonly bookingCounts = signal<Map<string, number>>(new Map());

  readonly showBookingDetailsModal = signal(false);
  readonly selectedBookingDetails = signal<BookingDetailsModalData | null>(null);
  readonly bookingDetailsLoading = signal(false);
  readonly bookingDetailsError = signal<string | null>(null);

  private mapToModalData(d: BookingDetailsForOwnerDto): BookingDetailsModalData {
    return {
      bookingID: d.bookingID,
      startDate: d.startDate,
      endDate: d.endDate,
      totalPrice: d.totalPrice,
      bookingStatus: d.bookingStatus,
      property: d.property,
      person: { label: 'Guest', name: d.guest.name, email: d.guest.email },
    };
  }

  readonly createForm = this.fb.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    location: ['', Validators.required],
    pricePerNight: [null as number | null, [Validators.required, Validators.min(1)]],
    maxGuests: [null as number | null, [Validators.required, Validators.min(1)]],
    imageUrl: [''],
  });

  readonly editForm = this.fb.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    location: ['', Validators.required],
    pricePerNight: [null as number | null, [Validators.required, Validators.min(1)]],
    maxGuests: [null as number | null, [Validators.required, Validators.min(1)]],
    imageUrl: [''],
  });

  ngOnInit(): void {
    this.loadProperties();
  }

  openModal(): void {
    this.createForm.reset();
    this.submitError.set(null);
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  openEditModal(property: Property): void {
    this.editingProperty.set(property);
    this.editForm.setValue({
      title: property.title,
      description: property.description,
      location: property.location,
      pricePerNight: property.pricePerNight,
      maxGuests: property.maxGuests,
      imageUrl: property.imageUrl ?? '',
    });
    this.editSubmitError.set(null);
    this.showEditModal.set(true);
  }

  closeEditModal(): void {
    this.showEditModal.set(false);
    this.editingProperty.set(null);
  }

  hasEditChanges(): boolean {
    const property = this.editingProperty();
    if (!property) return false;
    const val = this.editForm.getRawValue();
    return (
      val.title !== property.title ||
      val.description !== property.description ||
      val.location !== property.location ||
      val.pricePerNight !== property.pricePerNight ||
      val.maxGuests !== property.maxGuests ||
      (val.imageUrl ?? '') !== (property.imageUrl ?? '')
    );
  }

  submitUpdate(): void {
    const property = this.editingProperty();
    if (this.editForm.invalid || this.editSubmitting() || !property) return;

    const val = this.editForm.getRawValue();

    this.editSubmitting.set(true);
    this.editSubmitError.set(null);

    this.propertyService.update(property.propertyID, {
      title: val.title!,
      description: val.description!,
      location: val.location!,
      pricePerNight: val.pricePerNight!,
      maxGuests: val.maxGuests!,
      imageUrl: val.imageUrl ?? '',
    }).subscribe({
      next: (updated) => {
        this.properties.update(list =>
          list.map(p => p.propertyID === updated.propertyID ? updated : p)
        );
        this.editSubmitting.set(false);
        this.showEditModal.set(false);
        this.editingProperty.set(null);
      },
      error: () => {
        this.editSubmitError.set('Failed to update property. Please try again.');
        this.editSubmitting.set(false);
      },
    });
  }

  askDeleteProperty(propertyId: string): void {
    this.pendingDeleteId.set(propertyId);
    this.showDeleteConfirm.set(true);
  }

  cancelDelete(): void {
    this.showDeleteConfirm.set(false);
    this.pendingDeleteId.set(null);
  }

  confirmDelete(): void {
    const propertyId = this.pendingDeleteId();
    if (!propertyId || this.deletingId()) return;

    const title = this.properties().find(p => p.propertyID === propertyId)?.title ?? 'Property';
    this.showDeleteConfirm.set(false);
    this.deletingId.set(propertyId);

    this.propertyService.delete(propertyId).subscribe({
      next: () => {
        this.properties.update(list => list.filter(p => p.propertyID !== propertyId));
        this.deletingId.set(null);
        this.pendingDeleteId.set(null);
        this.deleteSuccessTitle.set(title);
        setTimeout(() => this.deleteSuccessTitle.set(null), 3000);
      },
      error: () => {
        this.deletingId.set(null);
        this.pendingDeleteId.set(null);
      },
    });
  }

  submitCreate(): void {
    if (this.createForm.invalid || this.submitting()) return;

    const user = this.authService.currentUser()!;
    const val = this.createForm.getRawValue();

    this.submitting.set(true);
    this.submitError.set(null);

    this.propertyService.create({
      ownerID: user.userID,
      title: val.title!,
      description: val.description!,
      location: val.location!,
      pricePerNight: val.pricePerNight!,
      maxGuests: val.maxGuests!,
      imageUrl: val.imageUrl ?? '',
    }).subscribe({
      next: (created) => {
        this.properties.update(list => [created, ...list]);
        this.submitting.set(false);
        this.showModal.set(false);
      },
      error: () => {
        this.submitError.set('Failed to create property. Please try again.');
        this.submitting.set(false);
      },
    });
  }

  openBookingsModal(property: Property): void {
    this.selectedPropertyForBookings.set(property);
    this.propertyBookings.set([]);
    this.bookingsError.set(null);
    this.bookingsLoading.set(true);
    this.showBookingsModal.set(true);
    this.bookingService.getPropertyBookings(property.propertyID).subscribe({
      next: (data) => {
        this.propertyBookings.set(data);
        this.bookingsLoading.set(false);
        this.bookingCounts.update(map => {
          const next = new Map(map);
          next.set(property.propertyID, data.length);
          return next;
        });
      },
      error: () => {
        this.bookingsError.set('Failed to load bookings for this property.');
        this.bookingsLoading.set(false);
      },
    });
  }

  closeBookingsModal(): void {
    this.showBookingsModal.set(false);
    this.selectedPropertyForBookings.set(null);
  }

  confirmBooking(bookingID: string): void {
    this.confirmingId.set(bookingID);
    this.bookingService.confirmBooking(bookingID).subscribe({
      next: () => {
        this.propertyBookings.update(list =>
          list.map(b => b.bookingID === bookingID ? { ...b, bookingStatus: 2 } : b)
        );
        this.confirmingId.set(null);
        this.confirmSuccessId.set(bookingID);
        setTimeout(() => this.confirmSuccessId.set(null), 4000);
      },
      error: () => {
        this.confirmingId.set(null);
        this.confirmErrorMessage.set('Failed to confirm booking. Please try again.');
        setTimeout(() => this.confirmErrorMessage.set(null), 4000);
      },
    });
  }

  finishBooking(bookingID: string): void {
    this.finishingId.set(bookingID);
    this.bookingService.finishBooking(bookingID).subscribe({
      next: () => {
        this.propertyBookings.update(list =>
          list.map(b => b.bookingID === bookingID ? { ...b, bookingStatus: 4 } : b)
        );
        this.finishingId.set(null);
        this.finishSuccessId.set(bookingID);
        setTimeout(() => this.finishSuccessId.set(null), 4000);
      },
      error: () => {
        this.finishingId.set(null);
        this.finishErrorMessage.set('Failed to finish booking. Please try again.');
        setTimeout(() => this.finishErrorMessage.set(null), 4000);
      },
    });
  }

  openBookingDetails(bookingID: string): void {
    this.selectedBookingDetails.set(null);
    this.bookingDetailsError.set(null);
    this.bookingDetailsLoading.set(true);
    this.showBookingDetailsModal.set(true);
    this.bookingService.getBookingDetailsForOwner(bookingID).subscribe({
      next: (data) => {
        this.selectedBookingDetails.set(this.mapToModalData(data));
        this.bookingDetailsLoading.set(false);
      },
      error: () => {
        this.bookingDetailsError.set('Failed to load booking details.');
        this.bookingDetailsLoading.set(false);
      },
    });
  }

  closeBookingDetails(): void {
    this.showBookingDetailsModal.set(false);
  }

  cancelBooking(bookingID: string): void {
    if (!confirm('Are you sure you want to cancel this booking?')) {
      return;
    }
    this.bookingService.cancelBooking(bookingID).subscribe({
      next: () => {
        this.propertyBookings.update(bookings => bookings.map(b => b.bookingID === bookingID ? { ...b, bookingStatus: 3 } : b));
        if (this.selectedBookingDetails()?.bookingID === bookingID) {
          this.selectedBookingDetails.update(details => details ? { ...details, bookingStatus: 3 } : details);
        } else {
          this.selectedBookingDetails.set(null);
        }
        this.cancelSuccessId.set(bookingID);
        setTimeout(() => this.cancelSuccessId.set(null), 4000);
      },
      error: (err) => {
        const msg = err?.error?.message ?? 'Failed to cancel booking. Please try again.';
        this.cancelErrorMessage.set(msg);
        setTimeout(() => this.cancelErrorMessage.set(null), 4000);
      },
    });
  }

  statusLabel(status: number): string {
    return STATUS_LABELS[status] ?? 'Unknown';
  }

  statusClass(status: number): string {
    return STATUS_CLASSES[status] ?? 'bg-gray-100 text-gray-600';
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }

  nights(start: string, end: string): number {
    const ms = new Date(end).getTime() - new Date(start).getTime();
    return Math.round(ms / (1000 * 60 * 60 * 24));
  }  

  avgRating(property: Property): number {
    const reviews = property.reviews;
    if (!reviews?.length) return 0;
    return reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length;
  }

  private loadProperties(): void {
    this.propertyService.getMyProperties().subscribe({
      next: (data) => {
        this.properties.set(data);
        this.loading.set(false);
        this.loadBookingCounts(data.map(p => p.propertyID));
      },
      error: () => {
        this.error.set('Failed to load your properties. Please try again.');
        this.loading.set(false);
      },
    });
  }

  private loadBookingCounts(propertyIDs: string[]): void {
    propertyIDs.forEach(id => {
      this.bookingService.getPropertyBookings(id).subscribe({
        next: (bookings) => {
          this.bookingCounts.update(map => {
            const next = new Map(map);
            next.set(id, bookings.length);
            return next;
          });
        },
        error: () => {},
      });
    });
  }
}
