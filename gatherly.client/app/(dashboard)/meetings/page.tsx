"use client"
import React, { useEffect, useState } from "react";
import { DatePicker, DatesProvider } from "@mantine/dates";
import 'dayjs/locale/pl';
import {Button, Divider, Group, HoverCard, Indicator, Paper, rem, Title} from "@mantine/core";
import classes from "./Meetings.module.css";
import '@mantine/dates/styles.css';
import Link from "next/link";
import { useWindowResize } from '@/lib/utils/useWindowResize';
import dayjs from "dayjs";
import 'dayjs/locale/pl';
import MeetingsGetMonth from "@/lib/widgets/Meetings/MeetingsGetByMonth";
import adjustTimeToLocal from "@/lib/widgets/Meetings/adjustTimeToLocal";
import {IconCalendarPlus} from "@tabler/icons-react";
import {openModal} from "@mantine/modals";
import NewMeeting from "@/components/dashboard/NewMeeting/NewMeeting";
dayjs.locale('pl');


export default function Meetings() {
    const [selected, setSelected] = useState<Date[]>([]);
    const [dateInfoCache, setDateInfoCache] = useState<Record<string, any[]>>({});
    const width = useWindowResize(7);

    const handleOpenModal = () => {
        openModal({
            title: 'Nowe spotkanie',
            size: '70%',
            radius: 10,
            children: (
                <NewMeeting/>
            ),
        });
    };

    useEffect(() => {
        async function fetchData() {
            const mtgs = await MeetingsGetMonth();
            const cache = mtgs.reduce((acc, meeting) => {
                const dateKey = dayjs(adjustTimeToLocal(meeting.date)).format('DD-MM-YYYY');
                if (!acc[dateKey]) acc[dateKey] = [];
                acc[dateKey].push(meeting);
                return acc;
            }, {} as Record<string, any[]>);

            setDateInfoCache(cache);
        }
        fetchData();
    }, []);


    const getDayInfo = (date: Date) => {
        const dateKey = dayjs(adjustTimeToLocal(date)).format('DD-MM-YYYY');
        return dateInfoCache[dateKey] || [];
    };

    return (
        <div className={classes.whole}>
            <Group className={classes.head}>
                <div className={classes.name}>
                    <Title order={2}>Kalendarz spotkań</Title>
                    Przejrzyj zaplanowane spotkania
                </div>
                <Button className={classes.buttonHead} variant={"outline"} rightSection={<IconCalendarPlus size={14}/>}
                        onClick={() => handleOpenModal()}>Utwórz spotkanie </Button>
            </Group>
            <Paper radius="lg" shadow="lg" p="lg" className={classes.calendarWrapper}>
                <DatesProvider settings={{ locale: 'pl', firstDayOfWeek: 1, weekendDays: [0, 6], timezone: 'UTC' }}>
                    <DatePicker
                        size={'xl'}
                        type="multiple"
                        defaultDate={new Date()}
                        classNames={{ calendarHeader: classes.calendarHeader }}
                        styles={(theme) => ({
                            display: 'flex', justifyContent: 'center', alignItems: 'center', day: {
                                borderRadius: 10,
                                flexGrow: 1,
                                height: rem(100),
                                width: `${width}px`,
                                fontSize: theme.fontSizes.xl,
                            },
                        })}
                        getDayProps={(date) => ({
                            selected: selected.some((s) => dayjs(adjustTimeToLocal(date)).isSame(s, 'date')),
                        })}
                        renderDay={(date) => {
                            const day = date.getDate();
                            const exampleDateInfo = getDayInfo(date);
                            const isExampleDate = exampleDateInfo.length > 0;
                            const eventCount = exampleDateInfo.length;

                            return (
                                // @ts-ignore - className działa
                                <HoverCard className={classes.calendarDay} withArrow openDelay={200} closeDelay={400}>
                                    <HoverCard.Target>
                                        <div style={{ width: '100%', position: 'relative' }}>
                                            {isExampleDate && (
                                                <Indicator zIndex={2} size={15} color="grape" offset={-30} inline
                                                           label={eventCount > 1 ? eventCount : ''} />
                                            )}
                                            {day}
                                        </div>
                                    </HoverCard.Target>
                                    {isExampleDate && (
                                        <HoverCard.Dropdown className={classes.hoverCard}>
                                            {exampleDateInfo.map(event => (
                                                <div key={event.id} className={classes.eventInfo}>
                                                    <Title order={2}>{event.name}</Title>
                                                    <div>{dayjs(adjustTimeToLocal(event.date)).format('DD.MM | HH:mm')}</div>
                                                    <div>{event.place}</div>
                                                    <Button component={Link} href={"/meeting/" + event.id}>
                                                        Przejdź do wydarzenia
                                                    </Button>
                                                    {eventCount > 1 && <Divider my="md" />}
                                                </div>
                                            ))}
                                        </HoverCard.Dropdown>
                                    )}
                                </HoverCard>
                            );
                        }}
                    />
                </DatesProvider>
            </Paper>
        </div>
    );
};
