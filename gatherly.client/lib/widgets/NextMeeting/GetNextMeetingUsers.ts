import axiosInstance from '@/lib/utils/AxiosInstance';
import {Person} from "@/lib/interfaces/types";

async function GetNextMeetingUsers(meetingId:string) : Promise<Person[]> {

    try {
        const response = await axiosInstance.get<Person[]>('Meetings/all/'+meetingId);
        return response.data.map(item => ({
            invitationId: undefined,
            //@ts-ignore
            personId: item.Id,
            //@ts-ignore
            name: item.Name,
            //@ts-ignore
            avatar: item.Avatar
        }));
    } catch (error) {
        throw error;
    }
}

export default GetNextMeetingUsers;

