import axiosInstance from '@/lib/utils/AxiosInstance';

async function DeleteUserMeeting(meetingId: string, userId: string) {
    try {
        const response = await axiosInstance.delete('Meetings/meeting/deleteUser', {
            data: {
                userId: userId,
                meetingId: meetingId
            }
        });
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default DeleteUserMeeting;
