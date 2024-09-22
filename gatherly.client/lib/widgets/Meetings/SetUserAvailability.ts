import axiosInstance from '@/lib/utils/AxiosInstance';

async function SetUserAvailability(meetingId : string, availability : string) {
    try {
        const response = await axiosInstance.put('Meetings/meeting/setAvailability?meetingId=' + meetingId+ '&availability=' +availability);
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default SetUserAvailability;

