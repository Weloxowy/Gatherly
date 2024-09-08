import dayjs from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';

dayjs.extend(utc);
dayjs.extend(timezone);

export default function adjustTimeToLocal(time: Date): string {
    const systemTimezone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    const localTime = dayjs(time).tz(systemTimezone);
    return localTime.format('YYYY-MM-DDTHH:mm:ss');
}
