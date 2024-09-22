'use client';
import React, {useEffect, useState} from 'react';
import {Box, Button, ColorSwatch, Group, ScrollArea, Table, Text} from '@mantine/core';
import dayjs from 'dayjs';
import {closeAllModals} from "@mantine/modals";
import {addNotification} from "@/lib/utils/notificationsManager";
import GetUserAvailability from "@/lib/widgets/Meetings/GetUserAvailability";
import getPageName from "@/lib/utils/qrGenerator/getPageName";
import SetUserAvailability from "@/lib/widgets/Meetings/SetUserAvailability";

interface MeetingDateTimeProps {
    startDateTime: Date;
    endDateTime: Date;
    intervalsPerHour: number;
}

const SetMeetingTimeByUser: React.FC<MeetingDateTimeProps> = ({startDateTime, endDateTime, intervalsPerHour}) => {
    const [selectedSlots, setSelectedSlots] = useState<Set<string>>(new Set());
    const [isSelecting, setIsSelecting] = useState<boolean>(false);
    const [days, setDays] = useState<number>(0);
    const link = getPageName(window.location.href)?.toString();
    useEffect(() => {
        const start = dayjs(startDateTime);
        const end = dayjs(endDateTime);
        const numberOfDays = end.diff(start, 'day') + 1;
        setDays(numberOfDays);
    }, [startDateTime, endDateTime]);

    useEffect(() => {
        const getAvailbility = async () => {
            try {
                if (link) {
                    const response = await GetUserAvailability(link);
                    if (response) {
                        console.log(response);
                        const availabilityString = response;
                        const newSelectedSlots = new Set<string>();
                        let index = 0;

                        for (let day = 0; day < days; day++) {
                            for (let interval = 0; interval < intervalsPerHour * 24; interval++) {
                                if (isSlotWithinRange(day, interval)) {
                                    if (index < availabilityString.length && availabilityString[index] === '1') {
                                        newSelectedSlots.add(`${day}-${interval}`);
                                    }
                                    index++;
                                }
                            }
                        }

                        setSelectedSlots(newSelectedSlots);
                    } else {
                        addNotification({
                            title: 'Wystąpił błąd',
                            message: 'Wystąpił błąd przy pobieraniu czasu spotkania z serwera. Spróbuj ponownie.',
                            color: 'red',
                        });
                    }
                }
            } catch (error) {
                addNotification({
                    title: 'Wystąpił błąd',
                    message: 'Ładowanie nie zostało zakończone pomyślnie.',
                    color: 'red',
                });
            }
        };

        getAvailbility();
    }, [link, days, intervalsPerHour]);
    const handleSubmit = async () => {
        let availabilityArray: string[] = [];
        for (let day = 0; day < days; day++) {
            for (let interval = 0; interval < intervalsPerHour * 24; interval++) {
                if (isSlotWithinRange(day, interval)) {
                    const slotKey = `${day}-${interval}`;
                    if (selectedSlots.has(slotKey)) {
                        availabilityArray.push("1");
                    } else {
                        availabilityArray.push("0");
                    }
                }
            }
        }
        const availabilityString = availabilityArray.join('');
        if (link) {
            try {
                await SetUserAvailability(link, availabilityString);
                addNotification({
                    title: 'Sukces',
                    message: 'Zmiany zostały pomyślnie zapisane.',
                    color: 'green',
                });
            } catch {
                addNotification({
                    title: 'Wystąpił błąd',
                    message: 'Ładowanie nie zostało zakończone pomyślnie.',
                    color: 'red',
                });
            }
        }
    };

    const handleRejectChanges = () => {
        closeAllModals();
    };

    const toggleSlot = (day: number, time: number) => {
        const slotKey = `${day}-${time}`;
        setSelectedSlots((prev) => {
            const newSelectedSlots = new Set(prev);
            if (newSelectedSlots.has(slotKey)) {
                newSelectedSlots.delete(slotKey);
            } else {
                newSelectedSlots.add(slotKey);
            }
            return newSelectedSlots;
        });
    };

    const handleMouseDown = (day: number, time: number) => {
        if (isSlotWithinRange(day, time)) {
            setIsSelecting(true);
            toggleSlot(day, time);
        }
    };

    const handleMouseOver = (day: number, time: number) => {
        if (isSelecting && isSlotWithinRange(day, time)) {
            toggleSlot(day, time);
        }
    };

    const handleMouseUp = () => {
        setIsSelecting(false);
    };

    const isSlotWithinRange = (day: number, time: number) => {
        const slotStartDate = dayjs(startDateTime)
            .add(day, 'day')
            .startOf('day')
            .add(Math.floor(time / intervalsPerHour), 'hour')
            .add((time % intervalsPerHour) * (60 / intervalsPerHour), 'minute');
        return (
            (slotStartDate.isSame(startDateTime) || slotStartDate.isAfter(startDateTime)) &&
            (slotStartDate.isSame(endDateTime) || slotStartDate.isBefore(endDateTime))
        );};

    const renderTimeSlots = () => {
        const timeSlots = [];
        const hours = 24;
        for (let hour = 0; hour < hours; hour++) {
            for (let interval = 0; interval < intervalsPerHour; interval++) {
                const time = hour * intervalsPerHour + interval;
                const displayTime = `${hour.toString().padStart(2, '0')}:${(interval * (60 / intervalsPerHour)).toString().padStart(2, '0')}`;
                timeSlots.push(
                    <tr key={time}>
                        <td>
                            <Text size="xs" style={{ alignItems: 'center' }}>
                                {displayTime}
                            </Text>
                        </td>
                        {Array.from({ length: days }).map((_, day) => {
                            const slotKey = `${day}-${time}`;
                            const withinRange = isSlotWithinRange(day, time);
                            return (
                                <td
                                    key={day}
                                    onMouseDown={() => handleMouseDown(day, time)}
                                    onMouseOver={() => handleMouseOver(day, time)}
                                    onMouseUp={handleMouseUp}
                                    style={{
                                        cursor: withinRange ? 'pointer' : 'not-allowed',
                                        backgroundColor: selectedSlots.has(slotKey) ? '#228be6' : withinRange ? '#f1f3f5' : '#e0e0e0',
                                        border: '1px solid #ced4da',
                                        borderRadius: '2px',
                                        height: '30px',
                                    }}
                                />
                            );
                        })}
                    </tr>
                );
            }
        }
        return timeSlots;
    };

    const renderDayHeaders = () => {
        return Array.from({ length: days }).map((_, day) => (
            <th key={day}>
                <Text style={{ alignItems: 'center' }}>{dayjs(startDateTime).add(day, 'day').format('DD.MM')}</Text>
            </th>
        ));
    };

    return (
        <ScrollArea style={{ width: '100%', height: '400px' }}>
            <Box onMouseLeave={handleMouseUp} style={{ display: 'flex', flexDirection: 'column' }}>
                <Table>
                    <thead>
                    <tr>
                        <th>
                            <Text style={{ alignItems: 'center' }}>Time</Text>
                        </th>
                        {renderDayHeaders()}
                    </tr>
                    </thead>
                    <tbody>{renderTimeSlots()}</tbody>
                </Table>
            </Box>
            <Group style={{paddingBottom:10, alignItems: "center", width: "100%" }}>
                <ColorSwatch color="#228be6" withShadow={false} /> Zaznaczone przez ciebie pasujące terminy spotkania
            </Group>
            <Button onClick={handleSubmit}>Zatwierdź zmiany</Button>
            <Button variant="outline" color="red" onClick={handleRejectChanges}>Odrzuć zmiany</Button>
        </ScrollArea>
    );
};

export default SetMeetingTimeByUser;
