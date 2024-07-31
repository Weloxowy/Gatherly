import axiosInstance from '@/lib/AxiosInstance';

async function sendOpeningOfRecovery(id: string) {

    try {
        return await axiosInstance.get('auth/recover/validate' + id);
    } catch (error) {
        throw error;
    }
}

export default sendOpeningOfRecovery;

