import axiosInstance from '@/lib/utils/AxiosInstance';
import {Meeting} from "@/lib/interfaces/types";
import dayjs from "dayjs";

async function MeetingsGet() : Promise<Meeting[]> {

    try {
        const response = await axiosInstance.get<Meeting[]>('Meetings/nextMeetings');
        return response.data.map(item => ({
            //@ts-ignore
            meetingId: item.id,
            //@ts-ignore
            date: dayjs(item.startOfTheMeeting).toDate(),
            //@ts-ignore
            name: item.meetingName,
            //@ts-ignore
            place: item.placeName,
            //@ts-ignore
            timezoneOffset: item.timeZone.baseUtcOffset,
            //@ts-ignore
            timezoneName: item.timeZone.displayName,
        }));
    } catch (error) {
        throw error;
    }
}
export default MeetingsGet;
