import React, { useState } from 'react';
import { Box, Table, ScrollArea, Text } from '@mantine/core';

const TimeScheduler = ({ days = 7, hours = 24, intervalsPerHour = 4 }) => {
    const [selectedSlots, setSelectedSlots] = useState(new Set());
    const [isSelecting, setIsSelecting] = useState(false);

    const toggleSlot = (day : number, time : number) => {
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
//@ts-ignore
    const handleMouseDown = (day, time) => {
        setIsSelecting(true);
        toggleSlot(day, time);
    };
//@ts-ignore
    const handleMouseOver = (day, time) => {
        if (isSelecting) {
            toggleSlot(day, time);
        }
    };

    const handleMouseUp = () => {
        setIsSelecting(false);
    };

    const renderTimeSlots = () => {
        const timeSlots = [];
        for (let hour = 0; hour < hours; hour++) {
            for (let interval = 0; interval < intervalsPerHour; interval++) {
                const time = hour * intervalsPerHour + interval;
                const displayTime = `${hour.toString().padStart(2, '0')}:${(interval * (60 / intervalsPerHour)).toString().padStart(2, '0')}`;
                timeSlots.push(
                    <tr key={time}>
                    <td>
                        <Text size="xs" //@ts-ignore
                            align="center">
                    {displayTime}
                    </Text>
                    </td>
                {Array.from({ length: days }).map((_, day) => {
                    const slotKey = `${day}-${time}`;
                    return (
                        <td
                            key={day}
                    onMouseDown={() => handleMouseDown(day, time)}
                    onMouseOver={() => handleMouseOver(day, time)}
                    onMouseUp={handleMouseUp}
                    style={{
                        cursor: 'pointer',
                            backgroundColor: selectedSlots.has(slotKey) ? '#228be6' : '#f1f3f5',
                            border: '1px solid #ced4da',
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
            <Text //@ts-ignore
                align="center">{`Day ${day + 1}`}</Text>
        </th>
    ));
    };

    return (
        <ScrollArea style={{ width: '100%', height: '400px' }}>
    <Box
        onMouseLeave={handleMouseUp}
    style={{ display: 'flex', flexDirection: 'column' }}
>
    <Table>
        <thead>
            <tr>
                <th>
                    <Text //@ts-ignore
                        align="center">Time</Text>
        </th>
    {renderDayHeaders()}
    </tr>
    </thead>
    <tbody>{renderTimeSlots()}</tbody>
    </Table>
    </Box>
    </ScrollArea>
);
};

export default TimeScheduler;
