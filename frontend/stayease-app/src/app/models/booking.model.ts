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

export interface MyBooking {
  bookingID: string;
  propertyID: string;
  userID: string;
  startDate: string;
  endDate: string;
  totalPrice: number;
  bookingStatus: number;
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




