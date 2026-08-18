import { Component, inject, signal, OnInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';

import { PropertyService } from '../../services/property.service';
import { Property, PropertySearchFilters } from '../../models/property.model';
import { BookingPropertySearchComponent } from '../../components/booking/booking-property-search';
import { PropertyReviewsModalComponent } from '../../components/booking/property-reviews-modal';
import { BookingFormModalComponent } from '../../components/booking/booking-form-modal';

@Component({
  selector: 'app-booking',
  imports: [DecimalPipe, BookingPropertySearchComponent, PropertyReviewsModalComponent, BookingFormModalComponent],
  templateUrl: './booking.html',
})
export class BookingComponent implements OnInit {
  private propertyService = inject(PropertyService);

  properties = signal<Property[]>([]);
  filteredProperties = signal<Property[]>([]);
  propertiesError = signal<string | null>(null);
  isSearching = signal(false);
  bookingProperty = signal<Property | null>(null);
  reviewsProperty = signal<Property | null>(null);

  ngOnInit(): void {
    this.propertyService.getAll().subscribe({
      next: (data) => {
        const normalized = data.map(p => ({ ...p, reviews: p.reviews ?? [] }));
        this.properties.set(normalized);
        this.filteredProperties.set(normalized);
      },
      error: () => this.propertiesError.set('Failed to load properties. Please try again later.'),
    });
  }

  onFiltersApplied(filters: PropertySearchFilters): void {
    const hasFilters = !!(filters.location || filters.minGuests || filters.maxGuests || filters.minPrice || filters.maxPrice || filters.checkInDate || filters.checkOutDate );

    if (!hasFilters) {
      this.filteredProperties.set(this.properties());
      this.propertiesError.set(null);
      return;
    }

    this.isSearching.set(true);
    this.propertiesError.set(null);

    this.propertyService.search(filters).subscribe({
      next: (data) => {
        this.filteredProperties.set(data.map(p => ({ ...p, reviews: p.reviews ?? [] })));
        this.isSearching.set(false);
      },
      error: () => {
        this.propertiesError.set('Failed to search properties. Please try again.');
        this.isSearching.set(false);
      },
    });
  }

  openBooking(property: Property): void {
    this.bookingProperty.set(property);
  }

  closeBooking(): void {
    this.bookingProperty.set(null);
  }

  openReviews(property: Property, event: Event): void {
    event.stopPropagation();
    this.reviewsProperty.set(property);
  }

  closeReviews(): void {
    this.reviewsProperty.set(null);
  }

  avgRating(property: Property): number {
    const reviews = property.reviews;
    if (!reviews?.length) return 0;
    return reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length;
  }
}
