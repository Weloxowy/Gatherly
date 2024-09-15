'use client';
import React, {useEffect, useState} from 'react';
import {Box, Button, ScrollArea, Table, Text} from '@mantine/core';
import dayjs from 'dayjs';
import {closeAllModals} from "@mantine/modals";

interface MeetingDateTimeProps {
    startDateTime: Date;
    endDateTime: Date;
    intervalsPerHour: number;
}

const SetMeetingTimeByUser: React.FC<MeetingDateTimeProps> = ({startDateTime, endDateTime, intervalsPerHour}) => {
    const [selectedSlots, setSelectedSlots] = useState<Set<string>>(new Set());
    const [isSelecting, setIsSelecting] = useState<boolean>(false);
    const [days, setDays] = useState<number>(0);

    const handleSubmit = () => {

    }

    const handleRejectChanges = () => {
        closeAllModals();
    };

    useEffect(() => {
        // Calculate the number of days between startDateTime and endDateTime
        const start = dayjs(startDateTime);
        const end = dayjs(endDateTime);
        const numberOfDays = end.diff(start, 'day') + 1; // +1 to include both start and end days
        setDays(numberOfDays);
    }, [startDateTime, endDateTime]);

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
        // Obliczamy czas rozpoczęcia dla danego dnia i czasu
        const slotStartDate = dayjs(startDateTime)
            .add(day, 'day')
            .startOf('day')
            .add(Math.floor(time / intervalsPerHour), 'hour')
            .add((time % intervalsPerHour) * (60 / intervalsPerHour), 'minute');

        // Sprawdzamy, czy slot jest równy lub po starcie i równy lub przed końcem
        return (
            (slotStartDate.isSame(startDateTime) || slotStartDate.isAfter(startDateTime)) &&
            (slotStartDate.isSame(endDateTime) || slotStartDate.isBefore(endDateTime))
        );};

    const renderTimeSlots = () => {
        const timeSlots = [];
        const hours = 24; // Godziny od 0 do 23
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
            <Button onClick={handleSubmit}>Zatwierdź zmiany</Button>
            <Button variant="outline" color="red" onClick={handleRejectChanges}>Odrzuć zmiany</Button>
        </ScrollArea>
    );
};

export default SetMeetingTimeByUser;
