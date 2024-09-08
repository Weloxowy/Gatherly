import axiosInstance from "@/lib/utils/AxiosInstance";


async function logoutUser() {
    try {
        const response = await axiosInstance.post('auth/logout', {}, {
        });

        return response.data;
    } catch (error) {
        throw error;
    }
    finally {
        window.location.href= "/";
    }
}

export default logoutUser;
