import { loginSchema, signupSchema } from '@/schemas/auth.schema';
import { UserProfile, VerifyEmail } from '../types/auth';
import { api } from './api.service';
import z from 'zod';

export const AuthService = {
  me: async () => {
    try {
      const res =  await api.get<UserProfile>('api/auth/me');
      return res.data;
    } catch (err) {
      throw err;
    }
  },
  login: async (data: z.infer<typeof loginSchema>) => {
    try {
      const res = await api.post<UserProfile>('api/auth/login', data);
      return res.data;
    } catch (err) {
      throw err;
    }
  },
  logout: async () => {
    await api.post('api/auth/logout');
  },
  signup: async (data: z.infer<typeof signupSchema>) => {
    try {
      const {confirmPassword, ...payload} = data; // Remove confirm password from payload
      const res = await api.post('api/auth/signup', payload);
      return res.data;
    } catch (err) {
      throw err;
    }
  },
  verifyEmail: async (data: VerifyEmail) => {
    try {
      const res = await api.post("api/auth/verify-email", data);
      return res.data;
    } catch (err) {
      throw err;
    }
  },
  resendOtp: async (email: string) => {
    try {
      const res = await api.post("api/auth/resend-otp", { email });
      return res.data;
    } catch (err) {
      throw err;
    }
  }
}
