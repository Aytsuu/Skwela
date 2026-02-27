export interface LoginRequest {
  username: string;
  password: string;
}

export interface SignupRequest extends LoginRequest { }

export interface UserProfile {
  userId: string
  email: string;
  displayName: string;
  displayImage?: string;
  role: string;
}