'use client';
import React, {useEffect, useMemo, useState} from 'react';
import {Box, Button, ColorSwatch, Group, ScrollArea, Space, Table, Text} from '@mantine/core';
import dayjs from 'dayjs';
import {closeAllModals} from "@mantine/modals";
import {addNotification} from "@/lib/utils/notificationsManager";
import GetAllUserAvailability from "@/lib/widgets/Meetings/GetAllUserAvailability";
import getPageName from "@/lib/utils/qrGenerator/getPageName";
import SetUserAvailability from "@/lib/widgets/Meetings/SetUserAvailability";

interface MeetingDateTimeProps {
    startDateTime: Date;
    endDateTime: Date;
    intervalsPerHour: number;
}

const SetMeetingTimeByUser: React.FC<MeetingDateTimeProps> = ({startDateTime, endDateTime, intervalsPerHour}) => {
    const [ownerUserAvailabilities, setOwnerUserAvailabilities] = useState<Set<string>>(new Set());
    const [participantsUserAvailabilities, setParticipantsUserAvailabilities] = useState<{
        [userId: string]: Set<string>
    }>({});
    const [userColors, setUserColors] = useState<{ [userId: string]: string }>({});
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
        const getAvailability = async () => {
            try {
                if (link) {
                    const response = await GetAllUserAvailability(link);
                    if (response) {
                        const newParticipantsUserAvailabilities: { [userId: string]: Set<string> } = {};
                        const newUserColors: { [userId: string]: string } = {};

                        response.forEach((user: { userId: string, availability: string, isOwner: boolean }) => {
                            const {userId, availability, isOwner} = user;
                            const userSlots = new Set<string>();

                            let index = 0;
                            for (let day = 0; day < days; day++) {
                                for (let interval = 0; interval < intervalsPerHour * 24; interval++) {
                                    if (isSlotWithinRange(day, interval)) {
                                        if (index < availability.length && availability[index] === '1') {
                                            userSlots.add(`${day}-${interval}`);
                                        }
                                        index++;
                                    }
                                }
                            }
                            newUserColors[userId] = getRandomColor();
                            if (!user.isOwner) {
                                newParticipantsUserAvailabilities[userId] = userSlots;
                            }
                            if (isOwner) {
                                setOwnerUserAvailabilities(userSlots);
                            }
                        });
                        setParticipantsUserAvailabilities(newParticipantsUserAvailabilities);
                        setUserColors(newUserColors);
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
                    title: 'Wystąpił błąd', message: 'Ładowanie nie zostało zakończone pomyślnie.', color: 'red',
                });
            }
        };
        getAvailability();
    }, [link, days, intervalsPerHour]);

    const handleSubmit = async () => {
        const availabilityArray: string[] = [];

        for (let day = 0; day < days; day++) {
            for (let interval = 0; interval < intervalsPerHour * 24; interval++) {
                if (isSlotWithinRange(day, interval)) {
                    const slotKey = `${day}-${interval}`;
                    availabilityArray.push(ownerUserAvailabilities.has(slotKey) ? "1" : "0");
                }
            }
        }

        const availabilityString = availabilityArray.join('');

        if (link) {
            try {
                await SetUserAvailability(link, availabilityString);
                addNotification({
                    title: 'Sukces', message: 'Zmiany zostały pomyślnie zapisane.', color: 'green',
                });
            } catch {
                addNotification({
                    title: 'Wystąpił błąd', message: 'Ładowanie nie zostało zakończone pomyślnie.', color: 'red',
                });
            }
        }
    };

    const handleRejectChanges = () => {
        if (confirm("Czy na pewno chcesz anulować zmiany?")) {
            closeAllModals();
        }
    };

    const toggleSlot = (day: number, time: number) => {
        const slotKey = `${day}-${time}`;
        setOwnerUserAvailabilities((prev) => {
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
        if (isSlotWithinRange(day, time) && ownerUserAvailabilities) {
            setIsSelecting(true);
            toggleSlot(day, time);
        }
    };

    const handleMouseOver = (day: number, time: number) => {
        if (isSelecting && isSlotWithinRange(day, time) && ownerUserAvailabilities) {
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

        return ((slotStartDate.isSame(startDateTime) || slotStartDate.isAfter(startDateTime)) && (slotStartDate.isSame(endDateTime) || slotStartDate.isBefore(endDateTime)));
    };

    const getRandomColor = () => {
        const letters = '0123456789ABCDEF';
        let color = '#';
        for (let i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        color += 'AD';
        return color;
    };

    const renderTimeSlots = useMemo(() => {
        const timeSlots = [];
        const hours = 24;
        for (let hour = 0; hour < hours; hour++) {
            for (let interval = 0; interval < intervalsPerHour; interval++) {
                const time = hour * intervalsPerHour + interval;
                const displayTime = `${hour.toString().padStart(2, '0')}:${(interval * (60 / intervalsPerHour)).toString().padStart(2, '0')}`;
                timeSlots.push(<tr key={time}>
                    <td>
                        <Text size="xs" style={{alignItems: 'center'}}>
                            {displayTime}
                        </Text>
                    </td>
                    {Array.from({length: days}).map((_, day) => {
                        const slotKey = `${day}-${time}`;
                        const withinRange = isSlotWithinRange(day, time);
                        const isOwnerSlot = ownerUserAvailabilities.has(slotKey);
                        const isParticipantSlot = Object.values(participantsUserAvailabilities).some(slots => slots.has(slotKey));

                        return (<td
                                key={day}
                                onMouseDown={() => handleMouseDown(day, time)}
                                //onMouseOver={() => handleMouseOver(day, time)}
                                onMouseUp={handleMouseUp}
                                style={{
                                    cursor: withinRange && ownerUserAvailabilities ? 'pointer' : 'not-allowed',
                                    backgroundColor: isOwnerSlot ? '#1373C5AD' : isParticipantSlot ? '#f1c40f' : withinRange ? '#f1f3f5' : '#e0e0e0',
                                    border: '1px solid #ced4da',
                                    borderRadius: '2px',
                                    height: '30px',
                                }}
                            />);
                    })}
                </tr>);
            }
        }
        return timeSlots;
    }, [days, ownerUserAvailabilities, participantsUserAvailabilities]);

    const renderDayHeaders = useMemo(() => (Array.from({length: days}).map((_, day) => (<th key={day}>
                <Text style={{alignItems: 'center'}}>{dayjs(startDateTime).add(day, 'day').format('DD/MM')}</Text>
            </th>))), [days, startDateTime]);

    return (<Box>
            <ScrollArea style={{height: '400px'}}>
                <Table>
                    <thead>
                    <tr>
                        <th>
                            <Text style={{alignItems: 'center'}}>Czas</Text>
                        </th>
                        {renderDayHeaders}
                    </tr>
                    </thead>
                    <tbody>{renderTimeSlots}</tbody>
                </Table>
            </ScrollArea>
            <Space h="md"/>
        <Group style={{paddingBottom:10, alignItems: "center", width: "100%" }}>
            <ColorSwatch color="#1373C5" withShadow={false} /> Zaznaczone przez ciebie pasujące terminy spotkania
            <ColorSwatch color="#f1c40f" withShadow={false} /> Zaznaczone przez innych pasujące terminy spotkania
        </Group>
        <Button onClick={handleSubmit}>Zapisz</Button>
            <Button onClick={handleRejectChanges}>Anuluj</Button>
        </Box>);
};

export default SetMeetingTimeByUser;
