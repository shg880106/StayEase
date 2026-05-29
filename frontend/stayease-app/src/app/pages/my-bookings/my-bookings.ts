import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { NgClass } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Button } from 'primeng/button';
import { BookingService } from '../../services/booking.service';
import { MyBooking, BookingDetails } from '../../models/booking.model';
import { BookingDetailsModalComponent, BookingDetailsModalData } from '../../components/booking/booking-details-modal';

const STATUS_LABELS: Record<number, string> = {
  1: 'Pending',
  2: 'Confirmed',
  3: 'Cancelled',
};

const STATUS_CLASSES: Record<number, string> = {
  1: 'bg-yellow-100 text-yellow-700',
  2: 'bg-green-100 text-green-700',
  3: 'bg-red-100 text-red-600',
};

@Component({
  selector: 'app-my-bookings',
  imports: [NgClass, RouterLink, BookingDetailsModalComponent, Button],
  templateUrl: './my-bookings.html',
})
export class MyBookingsComponent implements OnInit {
  private readonly bookingService = inject(BookingService);

  readonly bookings = signal<MyBooking[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly pendingBookings = computed(() => this.bookings().filter(b => b.bookingStatus === 1));
  readonly confirmedBookings = computed(() => this.bookings().filter(b => b.bookingStatus === 2));
  readonly cancelledBookings = computed(() => this.bookings().filter(b => b.bookingStatus === 3));

  readonly showDetailsModal = signal(false);
  readonly selectedDetails = signal<BookingDetailsModalData | null>(null);
  readonly detailsLoading = signal(false);
  readonly detailsError = signal<string | null>(null);

  readonly cancelSuccessId = signal<string | null>(null);
  readonly cancelErrorMessage = signal<string | null>(null);

  private mapToModalData(d: BookingDetails): BookingDetailsModalData {
    return {
      bookingID: d.bookingID,
      startDate: d.startDate,
      endDate: d.endDate,
      totalPrice: d.totalPrice,
      bookingStatus: d.bookingStatus,
      property: d.property,
      person: { label: 'Host', name: d.owner.name, email: d.owner.email },
    };
  }

  ngOnInit(): void {
    this.bookingService.getMyBookings().subscribe({
      next: (data) => {
        this.bookings.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load your bookings. Please try again.');
        this.loading.set(false);
      },
    });
  }

  openDetails(bookingID: string): void {
    this.selectedDetails.set(null);
    this.detailsError.set(null);
    this.detailsLoading.set(true);
    this.showDetailsModal.set(true);
    this.bookingService.getBookingDetails(bookingID).subscribe({
      next: (data) => {
        this.selectedDetails.set(this.mapToModalData(data));
        this.detailsLoading.set(false);
      },
      error: () => {
        this.detailsError.set('Failed to load booking details.');
        this.detailsLoading.set(false);
      },
    });
  }

  cancelBooking(bookingID: string): void {
    if (!confirm('Are you sure you want to cancel this booking?')) {
      return;
    }
    this.bookingService.cancelBooking(bookingID).subscribe({
      next: () => {
        this.bookings.update(bookings => bookings.map(b => b.bookingID === bookingID ? { ...b, bookingStatus: 3 } : b));
        if (this.selectedDetails()?.bookingID === bookingID) {
          this.selectedDetails.update(details => details ? { ...details, bookingStatus: 3 } : details);
        } else {
          this.selectedDetails.set(null);
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

  closeDetails(): void {
    this.showDetailsModal.set(false);
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
}
