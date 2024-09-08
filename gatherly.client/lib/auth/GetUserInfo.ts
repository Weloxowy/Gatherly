import axiosInstance from '@/lib/utils/AxiosInstance';

async function GetUserInfo() {

    try {
        const response = await axiosInstance.get('User/profile');
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default GetUserInfo;

