import axiosInstance from '@/lib/utils/AxiosInstance';

async function sendRecoveryRequest(email: string) {

    try {
        return await axiosInstance.post('auth/recover/send', email);
    } catch (error) {
        throw error;
    }
}

export default sendRecoveryRequest;

