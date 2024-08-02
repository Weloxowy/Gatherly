import axiosInstance from '@/lib/AxiosInstance';

async function sendReturnedCode(email: string, code: string) {
    try {
        const response = await axiosInstance.post('auth/login/code/verify', {
            email: email, code: code,
        });
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default sendReturnedCode;
