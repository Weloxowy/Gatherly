import axiosInstance from "@/lib/AxiosInstance";


async function logoutUser() {
    try {
        const response = await axiosInstance.post('auth/logout', {}, {
        });

        return response.data;
    } catch (error) {
        throw error;
    }
}

export default logoutUser;
