import axiosInstance from "@/lib/utils/AxiosInstance";

async function sendSsoCode(email: string) {
    try {
        const response = await axiosInstance.post('auth/login/code/send', email);
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default sendSsoCode;
