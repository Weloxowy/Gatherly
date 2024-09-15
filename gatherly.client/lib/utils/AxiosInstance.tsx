import axios, { AxiosError, AxiosResponse, InternalAxiosRequestConfig } from 'axios';
import RequestRefreshTokens from "@/lib/auth/RequestRefreshTokens";
import LogoutUser from "@/lib/auth/logoutUser";

const axiosInstance = axios.create({
    baseURL: process.env.API_ADDRESS,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true,
});

let isRefreshing = false;
let refreshTokenPromise: Promise<AxiosResponse> | null = null;

async function refreshAccessToken(): Promise<AxiosResponse> {
    try {
        const response = await RequestRefreshTokens();
        return response;
    } catch (error) {
        return Promise.reject(error);
    }
}

axiosInstance.interceptors.response.use(
    (response: AxiosResponse) => response,
    async (error: AxiosError) => {
        const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };
        if (originalRequest.url?.startsWith('auth/login') ||originalRequest.url?.startsWith('auth/register')) {
            return Promise.reject(error);
        }
        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            if (!isRefreshing) {
                isRefreshing = true;
                refreshTokenPromise = refreshAccessToken();
                try {
                    const response = await refreshTokenPromise;
                    isRefreshing = false;
                    refreshTokenPromise = null;
                    return axiosInstance(originalRequest);
                } catch (refreshError) {
                    isRefreshing = false;
                    refreshTokenPromise = null;
                    LogoutUser();
                    return Promise.reject(refreshError);
                }
            } else if (refreshTokenPromise) {
                try {
                    await refreshTokenPromise;
                    return axiosInstance(originalRequest);
                } catch (refreshError) {
                    return Promise.reject(refreshError);
                }
            }
        }
        return Promise.reject(error);
    }
);

export default axiosInstance;
