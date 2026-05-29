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

export interface CreatePropertyRequest {
  ownerID: string;
  title: string;
  description: string;
  pricePerNight: number;
  location: string;
  maxGuests: number;
  imageUrl: string;
}

export interface UpdatePropertyRequest {
  title: string;
  description: string;
  pricePerNight: number;
  location: string;
  maxGuests: number;
  imageUrl: string;
}

export interface PropertySearchFilters {
  location: string;
  minGuests: number | null;
  maxGuests: number | null;
  minPrice: number | null;
  maxPrice: number | null;
  checkInDate: string | null;
  checkOutDate: string | null;
}