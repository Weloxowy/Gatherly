import axiosInstance from '@/lib/utils/AxiosInstance';
import {ExtendedMeeting, Meeting} from "@/lib/interfaces/types";

async function GetMeeting(meetingId: string): Promise<ExtendedMeeting> {
    try {
        const response1 = await axiosInstance.get<ExtendedMeeting>('Meetings/' + meetingId);
        const response2 = await axiosInstance.get<Meeting[]>('Meetings/confirmed/' + meetingId);
        const response3 = await axiosInstance.get<Meeting[]>('Meetings/pending/' + meetingId);
        const response4 = await axiosInstance.get<Meeting[]>('Meetings/rejected/' + meetingId);

        // @ts-ignore
        return {
            id: response1.data.id,
            ownerId: response1.data.ownerId,
            startOfTheMeeting: new Date(response1.data.startOfTheMeeting),
            endOfTheMeeting: new Date(response1.data.endOfTheMeeting),
            isMeetingTimePlanned: response1.data.isMeetingTimePlanned,
            ownerName: response1.data.ownerName,
            creationTime: response1.data.creationTime,
            //@ts-ignore
            desc: response1.data.description,
            //@ts-ignore
            name: response1.data.meetingName,
            placeName: response1.data.placeName,
            lon: response1.data.lon ?? 0,
            lat: response1.data.lat ?? 0,
            //@ts-ignore
            timezoneOffset: response1.data.timeZone.baseUtcOffset,
            //@ts-ignore
            timezoneName: response1.data.timeZone.displayName,
            //@ts-ignore
            confirmedInvites: response2.data.map((invite: any) => ({
                invitationId: invite.id,
                personId: invite.UserId,
                name: invite.Name,
                avatar: invite.Avatar
            })),
            sendInvites: response3.data.map((invite: any) => ({
                invitationId: invite.id,
                personId: invite.UserId,
                name: invite.Name,
                avatar: invite.Avatar
            })),
            rejectedInvites: response4.data.map((invite: any) => ({
                invitationId: invite.id,
                personId: invite.UserId,
                name: invite.Name,
                avatar: invite.Avatar
            })),
            isRequestingUserAnOwner: response1.data.isRequestingUserAnOwner
        };
    } catch (error) {
        throw error;
    }
}

export default GetMeeting;
