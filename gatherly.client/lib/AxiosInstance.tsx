import axios, {AxiosError, AxiosResponse} from 'axios';

const axiosInstance = axios.create({
    baseURL: 'https://localhost:44329/api/', headers: {
        'Content-Type': 'application/json'
    }, withCredentials: true
});

// Interceptor
axiosInstance.interceptors.response.use((response: AxiosResponse) => {
    console.log('Response received:', response);
    return response;
}, (error: AxiosError) => {
    if (error.response) {
        console.error('Error response received:', error.response);
        return Promise.reject({
            data: error.response.data, status: error.response.status, code: error.response.status
        });
    } else {
        console.error('Network or other error:', error.message);
        return Promise.reject({
            data: error.message, status: 500, code: 500
        });
    }
});

export default axiosInstance;
