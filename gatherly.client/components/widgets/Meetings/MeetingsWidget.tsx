"use client"
import React, {useState} from "react";
import classes from "@/components/widgets/Notes/NotesWidget.module.css";
import {Button, Checkbox, Group, ScrollArea, Table, TextInput, Transition} from "@mantine/core";
import clsx from "clsx";
import {meetings} from "@/lib/meetingDetails/meetingDetails";
import dayjs from "dayjs";


const MeetingsWidget : React.FC = () => {
    const [scrolled, setScrolled] = useState(false);

    const rows = meetings.map((row) => (
        <Table.Tr key={row.id}>
            <Table.Td>{row.name}</Table.Td>
            <Table.Td>{dayjs(row.date).format('DD.MM.YYYY')}</Table.Td>
            <Table.Td>{dayjs(row.date).format('HH:mm')}</Table.Td>
            <Table.Td>{row.place}</Table.Td>
        </Table.Tr>
    ));

    return(
            <div className={classes.allmeetingslist}>
                <ScrollArea h={150} onScrollPositionChange={({ y }) => setScrolled(y !== 0)}>
                    <Table miw={700} stickyHeader>
                        <Table.Thead  className={clsx(classes.header, { [classes.scrolled]: scrolled })}>
                            <Table.Tr>
                                <Table.Th>Nazwa spotkania</Table.Th>
                                <Table.Th>Data</Table.Th>
                                <Table.Th>Godzina</Table.Th>
                                <Table.Th>Miejsce</Table.Th>
                            </Table.Tr>
                        </Table.Thead>
                        <Table.Tbody>{rows}</Table.Tbody>
                    </Table>
                </ScrollArea>
            </div>
    )
}

export default MeetingsWidget;
