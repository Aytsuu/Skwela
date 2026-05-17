import { loginSchema, signupSchema } from '@/schemas/auth.schema';
import { ResetPassword, UserProfile, VerifyEmail } from '../types/auth';
import { api } from './api.service';
import z from 'zod';
import { queryError } from '@/helpers/errorDisplay';

interface AuthApiResponse {
  userId: string;
  email: string;
  displayName?: string;
  displayImage?: string;
  name?: string;
  isAdmin?: boolean;
}

const mapUserProfile = (payload: AuthApiResponse): UserProfile => ({
  userId: payload.userId,
  email: payload.email,
  displayName: payload.displayName ?? payload.name ?? "",
  displayImage: payload.displayImage ?? "",
  isAdmin: payload.isAdmin ?? false
});

export const AuthService = {
  me: async () => {
    const res =  await api.get<AuthApiResponse>('api/auth/me');
    return mapUserProfile(res.data);
  },
  login: async (data: z.infer<typeof loginSchema>) => {
    try {
      const res = await api.post<AuthApiResponse>('api/auth/login', data);
      return mapUserProfile(res.data);
    } catch (err: unknown) {
      queryError(err);
      throw err;
    }
  },
  logout: async () => {
    await api.post('api/auth/logout');
  },
  signup: async (data: z.infer<typeof signupSchema>) => {
    try {
      const payload = {
        name: data.name,
        email: data.email,
        password: data.password
      };
      const res = await api.post('api/auth/signup', payload);
      return res.data;
    } catch (err: unknown) {
      queryError(err);
      throw err;
    }
  },
  verifyEmail: async (data: VerifyEmail) => {
    try {
      const res = await api.post<AuthApiResponse>("api/auth/verify-email", data);
      return mapUserProfile(res.data);
    } catch (err: unknown) {
      queryError(err);
      throw err;
    }
  },
  resendOtp: async (email: string) => {
    try {
      const res = await api.post("api/auth/resend-otp", { email });
      return res.data;
    } catch (err: unknown) {
      queryError(err);
      throw err;
    }
  },
  validateEmail: async (email: string) => {
    try {
      const res = await api.get(`api/auth/${email}/validate`);
      return res.data;
    } catch (err: unknown) {
      queryError(err);
      throw err;
    }
  },
  resetPassword: async (data: ResetPassword) => {
    try {
      const res = await api.patch(`api/auth/reset-password`, data);
      return res.data;
    } catch (err: unknown) {
      queryError(err);
      throw err;
    }
  }
}
