import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
dayjs.extend(utc);
dayjs.extend(timezone);
function adjustTimeToUTC(time: Date, meetingTimezone: string): string {
    const [hoursOffset, minutesOffset, secondsOffset] = meetingTimezone.split(':').map(Number);
    let adjustedTime = dayjs(time)
        .subtract(hoursOffset, 'hour')
        .subtract(minutesOffset, 'minute')
        .subtract(secondsOffset, 'second');
    return adjustedTime.format('YYYY-MM-DDTHH:mm:ss');
}

export default adjustTimeToUTC;
