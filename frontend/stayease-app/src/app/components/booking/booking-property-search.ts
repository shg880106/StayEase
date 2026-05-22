import { Component, input, output, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Property, PropertySearchFilters } from '../../models/property.model';

@Component({
  selector: 'app-booking-property-search',
  imports: [FormsModule],
  templateUrl: './booking-property-search.html',
})
export class BookingPropertySearchComponent {
  properties = input<Property[]>([]);

  filtersApplied = output<PropertySearchFilters>();

  selectedLocation = signal('');
  minGuests = signal<number | null>(null);
  maxGuests = signal<number | null>(null);
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);

  hasActiveFilters = computed(() =>
    !!this.selectedLocation() || !!this.minGuests() || !!this.maxPrice()
  );

  locations = computed(() =>
    [...new Set(this.properties().map((p) => p.location))].sort()
  );

  applyFilters(): void {
    this.filtersApplied.emit({
      location: this.selectedLocation(),
      minGuests: this.minGuests(),
      maxGuests: this.maxGuests(),
      minPrice: this.minPrice(),
      maxPrice: this.maxPrice(),
    });
  }

  clearFilters(): void {
    this.selectedLocation.set('');
    this.minGuests.set(null);
    this.maxGuests.set(null);
    this.minPrice.set(null);
    this.maxPrice.set(null);
    this.filtersApplied.emit({ location: '', minGuests: null, maxGuests: null, minPrice: null, maxPrice: null });
  }
}
