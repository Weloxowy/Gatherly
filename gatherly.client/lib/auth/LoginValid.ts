import axiosInstance from '@/components/AxiosInstance';
import saveTokenToLocalStorage from '@/lib/auth/headers/saveToLocalStorage';
import {isEmail} from "@mantine/form";

async function LoginValid(email:string, password:string) {

        try {
            const response = await axiosInstance.post(
                'auth/login/standard/verify',
                {
                    email: email,
                    password: password,
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

    export default LoginValid;

