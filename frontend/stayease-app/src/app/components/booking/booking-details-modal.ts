import { Component, input, output } from '@angular/core';
import { NgClass } from '@angular/common';

export interface BookingDetailsModalData {
  bookingID: string;
  startDate: string;
  endDate: string;
  totalPrice: number;
  bookingStatus: number;
  property: {
    propertyID: string;
    title: string;
    location: string;
    description: string;
    pricePerNight: number;
    imageUrl?: string;
  };
  person: {
    label: string;
    name: string;
    email: string;
  };
}

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
  selector: 'app-booking-details-modal',
  imports: [NgClass],
  templateUrl: './booking-details-modal.html',
})
export class BookingDetailsModalComponent {
  readonly loading = input<boolean>(false);
  readonly error = input<string | null>(null);
  readonly details = input<BookingDetailsModalData | null>(null);

  readonly closed = output<void>();

  close(): void {
    this.closed.emit();
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
