import axiosInstance from '@/lib/utils/AxiosInstance';
import {Meeting} from "@/lib/interfaces/types";
import dayjs from "dayjs";

async function MeetingsGet() : Promise<Meeting[]> {

    try {
        const response = await axiosInstance.get<Meeting[]>('Meetings/nextMeetings');
        //@ts-ignore
        const meetings: Meeting[] = response.data.map(item => ({
            //@ts-ignore
            id: item.id,
            //@ts-ignore
            date: dayjs(item.startOfTheMeeting).toDate(),
            //@ts-ignore
            name: item.meetingName,
            //@ts-ignore
            place: item.placeName
        }));

        return meetings;
    } catch (error) {
        throw error;
    }
}
export default MeetingsGet;
