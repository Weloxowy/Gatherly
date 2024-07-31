import axiosInstance from '@/lib/AxiosInstance';

async function sendRecoveryChanges(email: string, password: string) {

    try {
        return await axiosInstance.post('auth/recover/change', {
            "email": email, "newPassword": password
        });
    } catch (error) {
        throw error;
    }
}

export default sendRecoveryChanges;

