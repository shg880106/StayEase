import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { BookingService } from '../../services/booking.service';
import { ReviewService } from '../../services/review.service';
import { AuthService } from '../../services/auth.service';
import { BookingDetails } from '../../models/booking.model';

@Component({
  selector: 'app-review',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './review.html',
})
export class ReviewComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly bookingService = inject(BookingService);
  private readonly reviewService = inject(ReviewService);
  private readonly authService = inject(AuthService);

  readonly booking = signal<BookingDetails | null>(null);
  readonly loadingBooking = signal(true);
  readonly loadError = signal<string | null>(null);
  readonly isSubmitting = signal(false);
  readonly submitError = signal<string | null>(null);
  readonly submitSuccess = signal(false);
  readonly hoveredRating = signal(0);

  readonly reviewForm = this.fb.group({
    rating: [0, [Validators.required, Validators.min(1), Validators.max(5)]],
    comment: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(1000)]],
  });

  ngOnInit(): void {
    const bookingID = history.state?.bookingID as string | undefined;
    if (!bookingID) {
      this.loadError.set('No booking ID provided.');
      this.loadingBooking.set(false);
      return;
    }
    this.bookingService.getBookingDetails(bookingID).subscribe({
      next: (data) => {
        this.booking.set(data);
        this.loadingBooking.set(false);
      },
      error: () => {
        this.loadError.set('Failed to load booking details. Please try again.');
        this.loadingBooking.set(false);
      },
    });
  }

  setRating(value: number): void {
    this.reviewForm.patchValue({ rating: value });
    this.reviewForm.get('rating')?.markAsTouched();
  }

  get ratingErrors(): string | null {
    const ctrl = this.reviewForm.get('rating');
    if (!ctrl?.touched || !ctrl.invalid) return null;
    if (ctrl.hasError('min') || ctrl.hasError('required')) return 'Please select a rating.';
    return null;
  }

  get commentErrors(): string | null {
    const ctrl = this.reviewForm.get('comment');
    if (!ctrl?.touched || !ctrl.invalid) return null;
    if (ctrl.hasError('required')) return 'Comment is required.';
    if (ctrl.hasError('minlength')) return 'Comment must be at least 10 characters.';
    if (ctrl.hasError('maxlength')) return 'Comment cannot exceed 1000 characters.';
    return null;
  }

  onSubmit(): void {
    if (this.reviewForm.invalid) {
      this.reviewForm.markAllAsTouched();
      return;
    }
    const b = this.booking();
    const user = this.authService.currentUser();
    if (!b || !user) return;

    this.isSubmitting.set(true);
    this.submitError.set(null);

    const { rating, comment } = this.reviewForm.getRawValue();
    this.reviewService.createReview({
      userID: user.userID,
      propertyID: b.property.propertyID,
      bookingID: b.bookingID,
      rating: rating!,
      comment: comment!,
    }).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.submitSuccess.set(true);
        setTimeout(() => this.router.navigate(['/my-bookings']), 2000);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.submitError.set(err?.error?.message ?? 'Failed to submit review. Please try again.');
      },
    });
  }
}