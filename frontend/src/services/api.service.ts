import axios from 'axios';
import { AuthService } from './auth.service';

// API instance with credentials enabled
export const api = axios.create({
  baseURL: process.env.NODE_ENV == "development" ? "http://localhost:8080" : process.env.NEXT_PUBLIC_API_URL,
  withCredentials: true // This tells the browser to send the HttpOnly cookies
});

// Response Interceptor
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (originalRequest.url.includes('api/auth/me')) {
      return Promise.reject(error);
    }
    
    if (error?.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        await axios.post(`${api.defaults.baseURL}/api/auth/refresh-token`, {}, {
          withCredentials: true
        });
        return api(originalRequest);

      } catch (refreshError) {
        AuthService.logout();
        window.location.href = '/authentication/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);