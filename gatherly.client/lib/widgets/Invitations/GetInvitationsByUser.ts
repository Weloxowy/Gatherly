import axiosInstance from '@/lib/utils/AxiosInstance';
import { InvitationMeeting } from "@/lib/interfaces/types";

async function GetInvitationsByUser(): Promise<InvitationMeeting[]> {
    try {
        const response = await axiosInstance.get<InvitationMeeting[]>('Invitations/user');

        const invitationMeetings: InvitationMeeting[] = response.data.map(item => ({
            //@ts-ignore
            InvitationId: item.invitationId,
            userId: item.userId,
            meetingId: item.meetingId,
            validTime: new Date(item.validTime),
            //@ts-ignore
            OwnerId: item.ownerId,
            //@ts-ignore
            MeetingName: item.meetingName,
            //@ts-ignore
            Description: item.description,
            //@ts-ignore
            PlaceName: item.placeName,
            //@ts-ignore
            StartOfTheMeeting: new Date(item.startOfTheMeeting),
            //@ts-ignore
            EndOfTheMeeting: new Date(item.endOfTheMeeting),
            //@ts-ignore
            TimeZone: item.timeZone
        }));

        return invitationMeetings;
    } catch (error) {
        console.error("Error fetching invitations by user:", error);
        throw error;
    }
}

export default GetInvitationsByUser;
