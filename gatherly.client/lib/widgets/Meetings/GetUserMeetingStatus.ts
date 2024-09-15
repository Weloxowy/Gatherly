import axiosInstance from '@/lib/utils/AxiosInstance';
//@ts-ignore
async function GetUserMeetingStatus(data) {

    try {
        const response = await axiosInstance.post('Meetings/meeting/getStatus?meetingId=93e78220-173e-4de1-a83c-b1e800b10f8d');
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default GetUserMeetingStatus;

