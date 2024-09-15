import { Meeting } from "@/lib/interfaces/types";
import axiosInstance from "@/lib/utils/AxiosInstance";
import dayjs from "dayjs";
import adjustTimeToLocal from "@/lib/widgets/Meetings/adjustTimeToLocal";

async function MeetingsGetMonth(): Promise<Meeting[]> {
    try {
        const response = await axiosInstance.get<Meeting[]>('Meetings/allMeetings');
        //@ts-ignore
        const meetings: Meeting[] = response.data.map(item => ({
            //@ts-ignore
            id: item.id,
            //@ts-ignore
            date: dayjs(item.startOfTheMeeting).toDate(),
            //@ts-ignore
            name: item.meetingName,
            //@ts-ignore
            place: item.placeName,
            //@ts-ignore
            timezoneOffset: item.timeZone.baseUtcOffset,
            //@ts-ignore
            timezoneName: item.timeZone.displayName
        }));

        return meetings;
    } catch (error) {
        throw error;
    }
}

export default MeetingsGetMonth;
