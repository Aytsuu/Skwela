import axios from 'axios';

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
    const requestUrl = originalRequest?.url ?? "";
    const isAuthFlowRequest =
      requestUrl.includes('api/auth/me') ||
      requestUrl.includes('api/auth/login') ||
      requestUrl.includes('api/auth/logout') ||
      requestUrl.includes('api/auth/refresh-token');

    if (isAuthFlowRequest) {
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
        if (typeof window !== "undefined" && window.location.pathname !== "/authentication/login") {
          window.location.replace("/authentication/login");
        }
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);
