export interface VerifyEmail {
  email: string;
  otpCode: string;
  type: string;
}

export interface ResetPassword {
  email: string;
  password: string;
}

export interface UserProfile {
  userId: string;
  email: string;
  displayName: string;
  displayImage?: string;
  role: string;
}