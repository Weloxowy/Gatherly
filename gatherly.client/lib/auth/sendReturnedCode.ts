import axiosInstance from '@/components/AxiosInstance';
import saveTokenToLocalStorage from '@/lib/auth/headers/saveToLocalStorage';

async function sendReturnedCode(email: string, code: string) {
    try {
        const response = await axiosInstance.post(
            'auth/login/code/verify',
            {
                email: email,
                code: code,
            }
        );

        const responseData: AuthReturn = response.data;
        saveTokenToLocalStorage("Authorization", "Bearer " + responseData.jwt);
        saveTokenToLocalStorage("Refresh", responseData.refresh);
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default sendReturnedCode;
