import { LoginRequest, SignupRequest, UserProfile } from '../types/auth';
import { redirect } from 'next/navigation';
import { api } from './api.service';

export const AuthService = {
  me: async () => {
    try {
      const res =  await api.get<UserProfile>('api/auth/me');
      return res.data;
    } catch (err) {
      throw err;
    }
  },
  login: async (data: LoginRequest) => {
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
  signup: async (data: SignupRequest) => {
    try {
      const res = await api.post('api/auth/signup', data);
      return res.data;
    } catch (err) {
      throw err;
    }
  }
}
