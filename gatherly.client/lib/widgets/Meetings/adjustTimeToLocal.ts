﻿import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
dayjs.extend(utc);
dayjs.extend(timezone);

function adjustTimeToLocal(time: Date, meetingTimezone: string): string {
    const [hoursOffset, minutesOffset, secondsOffset] = meetingTimezone.split(':').map(Number);
    let adjustedTime = dayjs(time)
        .add(hoursOffset, 'hour')
        .add(minutesOffset, 'minute')
        .add(secondsOffset, 'second');
    return adjustedTime.format('YYYY-MM-DDTHH:mm:ss');
}
export default adjustTimeToLocal;

