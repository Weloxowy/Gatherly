import axiosInstance from "@/lib/AxiosInstance";
import {readFromLocalStorage} from "@/lib/auth/headers/readFromLocalStorage";

async function logoutUser() {
    try {
        const authorizationToken = readFromLocalStorage("Authorization");
        if (authorizationToken === null) {
            throw new Error("Authorization token not found");
        }
        const refreshToken = readFromLocalStorage("Refresh");
        const response = await axiosInstance.post('auth/logout', {}, {
            headers: {
                'Authorization': authorizationToken, 'Refresh': refreshToken
            }
        });
        localStorage.removeItem("Authorization");
        localStorage.removeItem("Refresh");

        return response.data;
    } catch (error) {
        throw error;
    }
}

export default logoutUser;
