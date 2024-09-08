import axiosInstance from '@/lib/utils/AxiosInstance';
async function RegisterUser(name: string, email: string, password: string) {

    try {
        const response = await axiosInstance.post('auth/register', {
            name: name, email: email, password: password
        });

        return response.data;
    } catch (error) {
        throw error;
    }
}

export default RegisterUser;

