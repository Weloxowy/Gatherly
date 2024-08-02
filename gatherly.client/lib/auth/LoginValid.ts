import axiosInstance from '@/lib/AxiosInstance';

async function LoginValid(email: string, password: string) {

    try {
        const response = await axiosInstance.post('auth/login/standard/verify', {
            email: email, password: password,
        });
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default LoginValid;

