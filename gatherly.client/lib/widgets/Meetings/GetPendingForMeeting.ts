import axiosInstance from '@/lib/utils/AxiosInstance';
import { Person } from "@/lib/interfaces/types";

async function GetPendingForMeeting(meetingId: string): Promise<Person[]> {
    try {
        const response = await axiosInstance.get(`Invitations/meeting/${meetingId}`);
        const mapToPerson = (meeting: any): Person => ({
            //id: meeting.id,
            personId: meeting.userId,
            name: meeting.userName,
            avatar: meeting.userAvatar
        });
        return response.data.map(mapToPerson);
    } catch (error) {
        console.error("Error in GetPendingForMeeting:", error);
        throw error;
    }
}

export default GetPendingForMeeting;
