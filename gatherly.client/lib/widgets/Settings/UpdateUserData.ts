import axiosInstance from '@/lib/utils/AxiosInstance';
//@ts-ignore
async function UpdateUserData(data) {

    try {
        const response = await axiosInstance.patch('User/profile',JSON.stringify(data));
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default UpdateUserData;

