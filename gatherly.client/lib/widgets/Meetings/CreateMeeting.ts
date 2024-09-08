import axiosInstance from '@/lib/utils/AxiosInstance';
//@ts-ignore
async function CreateMeeting(data) {

    try {
        const response = await axiosInstance.post('Meetings',JSON.stringify(data));
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default CreateMeeting;

