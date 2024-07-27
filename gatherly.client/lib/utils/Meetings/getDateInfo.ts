import dayjs from 'dayjs';
import { meetings } from '@/lib/meetingDetails/meetingDetails';

export const getDateInfo = (date: Date) => {
    return meetings.filter((exampleDate) =>
        dayjs(exampleDate.date).isSame(date, 'day')
    ).sort((a, b) => a.date.getTime() - b.date.getTime());
};
