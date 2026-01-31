export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
}

export interface SignupRequest extends LoginRequest { }

export interface UserProfile {
  userId: string;
  username: string;
  email: string;
  displayName: string;
  displayImage: string;
  role: string;
}