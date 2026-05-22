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




