import axiosInstance from '@/lib/utils/AxiosInstance';

async function GetUserAvailability(meetingId : string) {
    try {
        const response = await axiosInstance.get('Meetings/meeting/getAvailability/' + meetingId);
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default GetUserAvailability;

