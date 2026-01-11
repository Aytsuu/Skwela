import { LoginRequest, LoginResponse, SignupRequest } from '../types/auth';
import Cookies from 'js-cookie';
import { redirect } from 'next/navigation';
import { api } from './api.service';

export const AuthService = {
  login: async (data: LoginRequest) => {
    try {
      const res = await api.post<LoginResponse>('api/auth/login', data);
      return res.data;
    } catch (err) {
      throw err;
    }
  },
  logout: async () => {
    Cookies.remove('accessToken');
    Cookies.remove('refreshToken');
    redirect('/login')
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
