import axiosInstance from '@/lib/utils/AxiosInstance';

async function CalendarCardGet() {

    try {
        const response = await axiosInstance.get('Meetings/numberOfMeetings');
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default CalendarCardGet;

