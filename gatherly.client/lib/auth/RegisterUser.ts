import axiosInstance from '@/lib/AxiosInstance';
import saveTokenToLocalStorage from '@/lib/auth/headers/saveToLocalStorage';
import {AuthReturn} from "@/lib/interfaces/types";

async function RegisterUser(name: string, email: string, password: string) {

    try {
        const response = await axiosInstance.post('auth/register', {
            name: name, email: email, password: password
        });

        const responseData: AuthReturn = response.data;
        saveTokenToLocalStorage("Authorization", "Bearer " + responseData.jwt);
        saveTokenToLocalStorage("Refresh", responseData.refresh);
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default RegisterUser;

