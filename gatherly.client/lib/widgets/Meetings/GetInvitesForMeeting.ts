import axiosInstance from '@/lib/utils/AxiosInstance';
import { Person } from "@/lib/interfaces/types";

async function GetInvitesForMeeting(meetingId: string): Promise<Person[]> {
    try {
        const response = await axiosInstance.get(`Meetings/all/${meetingId}`);
        const mapToPerson = (meeting: any): Person => ({
            invitationId: undefined,
            personId: meeting.UserId,
            name: meeting.Name,
            avatar: meeting.Avatar
        });
        return response.data.map(mapToPerson);
    } catch (error) {
        console.error("Error in GetInvitesForMeeting:", error);
        throw error;
    }
}

export default GetInvitesForMeeting;
