export interface Review {
  reviewID: string;
  propertyID: string;
  userID: string;
  bookingID: string;
  rating: number;
  comment: string;
}

export interface CreateReviewRequest {
  userID: string;
  propertyID: string;
  bookingID: string;
  rating: number;
  comment: string;
}