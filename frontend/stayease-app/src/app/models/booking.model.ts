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

export interface Property {
  propertyID: string;
  userID: string;
  title: string;
  description: string;
  pricePerNight: number;
  location: string;
  maxGuests: number;
  imageUrl: string;
}
