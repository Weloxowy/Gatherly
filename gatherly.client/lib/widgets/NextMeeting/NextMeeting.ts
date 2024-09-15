import axiosInstance from '@/lib/utils/AxiosInstance';
import {Meeting} from "@/lib/interfaces/types";
import dayjs from "dayjs";

async function NextMeeting(): Promise<Meeting | null> {
    try {
        const response = await axiosInstance.get<Meeting>('Meetings/nextMeeting');

        if (!response.data || Object.keys(response.data).length === 0) {
            return null;
        }
        console.log(response);
        return {
            //@ts-ignore
            meetingId: response.data.id,
            //@ts-ignore
            date: dayjs(response.data.startOfTheMeeting).toDate(),
            //@ts-ignore
            name: response.data.meetingName,
            //@ts-ignore
            place: response.data.placeName,
            //@ts-ignore
            timezoneOffset: response.data.timeZone.baseUtcOffset,
            //@ts-ignore
            timezoneName: response.data.timeZone.displayName,
        };
    } catch (error) {
        throw error;
    }
}

export default NextMeeting;
