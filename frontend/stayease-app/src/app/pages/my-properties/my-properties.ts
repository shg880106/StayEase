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

  readonly showEditModal = signal(false);
  readonly editingProperty = signal<Property | null>(null);
  readonly editSubmitting = signal(false);
  readonly editSubmitError = signal<string | null>(null);
  readonly deletingId = signal<string | null>(null);

  readonly showDeleteConfirm = signal(false);
  readonly pendingDeleteId = signal<string | null>(null);
  readonly deleteSuccessTitle = signal<string | null>(null);

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
