import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PropertyService } from '../../services/property.service';
import { AuthService } from '../../services/auth.service';
import { Property } from '../../models/property.model';

@Component({
  selector: 'app-my-properties',
  imports: [ReactiveFormsModule],
  templateUrl: './my-properties.html',
})
export class MyPropertiesComponent implements OnInit {
  private readonly propertyService = inject(PropertyService);
  readonly authService = inject(AuthService);
  private readonly fb = inject(FormBuilder);

  readonly properties = signal<Property[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly showModal = signal(false);
  readonly submitting = signal(false);
  readonly submitError = signal<string | null>(null);

  readonly createForm = this.fb.group({
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

  private loadProperties(): void {
    this.propertyService.getMyProperties().subscribe({
      next: (data) => {
        this.properties.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load your properties. Please try again.');
        this.loading.set(false);
      },
    });
  }
}
