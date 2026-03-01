export interface VerifyEmail {
  email: string;
  otpCode: string;
}

export interface UserProfile {
  userId: string;
  email: string;
  displayName: string;
  displayImage?: string;
  role: string;
}