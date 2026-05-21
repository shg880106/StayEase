export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  userID: string;
  name: string;
  email: string;
}

export interface AuthUser {
  userID: string;
  name: string;
  email: string;
}
