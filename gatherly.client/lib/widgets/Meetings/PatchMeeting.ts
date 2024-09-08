import axiosInstance from '@/lib/utils/AxiosInstance';

//@ts-ignore
async function PatchMeeting(data,meetingId : string) {

    try {
        const response = await axiosInstance.patch('Meetings/'+meetingId,JSON.stringify(data));
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default PatchMeeting;

