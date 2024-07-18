import axiosInstance from "@/components/AxiosInstance";

async function sendSsoCode(email: string) {
    try {
        const response = await axiosInstance.post('auth/login/code/send', email);
        console.log('Request successful:', response);
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default sendSsoCode;
