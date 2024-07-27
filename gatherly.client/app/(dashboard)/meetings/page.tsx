"use client"
import React from "react";
import {DatePicker, DatesProvider} from "@mantine/dates";
import 'dayjs/locale/pl';
import {Button, Divider, HoverCard, Indicator, Paper, rem, Title} from "@mantine/core";
import classes from "./Meetings.module.css";
import '@mantine/dates/styles.css';
import Link from "next/link";
import {useWindowResize} from '@/lib/utils/Meetings/useWindowResize';
import {getDateInfo} from '@/lib/utils/Meetings/getDateInfo';
import dayjs from "dayjs";

export default function Meetings() {
    const [selected, setSelected] = React.useState<Date[]>([]);
    const width = useWindowResize(7);

    return (<div className={classes.whole}>
            <div className={classes.name}>
                <Title order={2}>Kalendarz spotkań</Title>
                Przejrzyj zaplanowane spotkania
            </div>
            <Paper radius="lg" shadow="lg" p="lg" className={classes.calendarWrapper}>
                <DatesProvider settings={{locale: 'pl', firstDayOfWeek: 1, weekendDays: [0, 6], timezone: 'UTC'}}>
                    <DatePicker
                        size={'xl'}
                        type="multiple"
                        defaultDate={new Date()}
                        classNames={{
                            calendarHeader: classes.calendarHeader
                        }}
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
                            selected: selected.some((s) => dayjs(date).isSame(s, 'date')),
                        })}
                        renderDay={(date) => {
                            const day = date.getDate();
                            const exampleDateInfo = getDateInfo(date);
                            const isExampleDate = exampleDateInfo.length > 0;
                            const eventCount = exampleDateInfo.length;

                            return (// @ts-ignore
                                <HoverCard className={classes.calendarDay} withArrow openDelay={200} closeDelay={400}>
                                    <HoverCard.Target>
                                        <div style={{width: '100%', position: 'relative'}}>
                                            {isExampleDate && (<Indicator size={15} color="grape" offset={-30} inline
                                                                          label={eventCount > 1 ? eventCount : ''}/>)}
                                            {day}
                                        </div>
                                    </HoverCard.Target>
                                    {isExampleDate && (<HoverCard.Dropdown className={classes.hoverCard}>
                                            {exampleDateInfo.map(event => (
                                                <div key={event.id} className={classes.eventInfo}>
                                                    <Title order={2}>{event.name}</Title>
                                                    <div>{event.date.toDateString()}</div>
                                                    <div>{event.place}</div>
                                                    <Button component={Link} href={"/meeting/" + event.id}>
                                                        Przejdź do wydarzenia
                                                    </Button>
                                                    {eventCount > 1 && <Divider my="md"/>}
                                                </div>))}
                                        </HoverCard.Dropdown>)}
                                </HoverCard>);
                        }}
                    />
                </DatesProvider>
            </Paper>
        </div>);
};
