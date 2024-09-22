import axiosInstance from '@/lib/utils/AxiosInstance';

async function GetAllUserAvailability(meetingId : string) {
    try {
        const response = await axiosInstance.get('Meetings/meeting/getAllAvailability/' + meetingId);
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default GetAllUserAvailability;

