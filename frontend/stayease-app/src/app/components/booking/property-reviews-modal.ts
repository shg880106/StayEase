import { Component, input, output } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { Property } from '../../models/property.model';

@Component({
  selector: 'app-property-reviews-modal',
  imports: [DecimalPipe],
  templateUrl: './property-reviews-modal.html',
})
export class PropertyReviewsModalComponent {
  readonly property = input.required<Property>();
  readonly closed = output<void>();

  close(): void {
    this.closed.emit();
  }

  get avgRating(): number {
    const reviews = this.property().reviews;
    if (!reviews?.length) return 0;
    return reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length;
  }

  stars(rating: number): boolean[] {
    return [1, 2, 3, 4, 5].map((s) => s <= Math.round(rating));
  }
}
