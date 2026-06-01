export interface CreateBookingRequest {
  propertyID: string;
  userID: string;
  startDate: string;
  endDate: string;
}

export interface BookingResponse {
  bookingID: string;
  totalPrice: number;
}

export interface BookingReviewSummary {
  rating: number;
  comment: string;
}

export interface MyBooking {
  bookingID: string;
  propertyID: string;
  userID: string;
  startDate: string;
  endDate: string;
  totalPrice: number;
  bookingStatus: number;
  canBeReviewed: boolean;
  review?: BookingReviewSummary;
}

export interface BookingDetails {
  bookingID: string;
  startDate: string;
  endDate: string;
  totalPrice: number;
  bookingStatus: number;
  property: PropertyDetails;
  owner: OwnerDetails;
}

export interface PropertyDetails {
  propertyID: string;
  title: string;
  location: string;
  description: string;
  pricePerNight: number;
  imageUrl?: string;
}

export interface OwnerDetails {
  name: string;
  email: string;
}

export interface BookingDetailsForOwnerDto {
  bookingID: string;
  startDate: string;
  endDate: string;
  totalPrice: number;
  bookingStatus: number;
  property: PropertyDetails;
  guest: GuestDetails;
}

export interface GuestDetails {
  name: string;
  email: string;
}




