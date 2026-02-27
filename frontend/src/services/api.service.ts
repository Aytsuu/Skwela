import axios from 'axios';
import Cookies from 'js-cookie';

// Api instance
export const api = axios.create({
  baseURL: "https://skwela.paoloaraneta.dev"
})

// Authorization interceptor
api.interceptors.request.use((config) => {
  const token = Cookies.get('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config
})

// Refresh token interceptor
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    if (error?.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        // Call refresh token
        const oldAccessToken = Cookies.get('accessToken');
        const oldRefreshToken = Cookies.get('refreshToken');

        const response = await axios.post(`${api.defaults.baseURL}/api/auth/refresh-token`, {
          accessToken: oldAccessToken,
          refreshToken: oldRefreshToken
        });

        const { accessToken, refreshToken } = response.data

        const sixtyMinutes = new Date(new Date().getTime() + 60 * 60 * 1000);
        Cookies.set('accessToken', accessToken, { expires: sixtyMinutes, path: '/' });
        Cookies.set('refreshToken', refreshToken, { path: '/' });

        originalRequest.headers.Authorization = `Bearer ${accessToken}`;
        return api(originalRequest);

      } catch (refreshError) {
        Cookies.remove('accessToken');
        Cookies.remove('refreshToken');
        window.location.href = '/authentication/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
)