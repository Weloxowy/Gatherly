import axiosInstance from '@/lib/utils/AxiosInstance';

async function RequestRefreshTokens() {

    try {
        const response = await axiosInstance.post('Tokens/refresh');
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default RequestRefreshTokens;

