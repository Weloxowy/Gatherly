"use client"
import React, {useEffect, useState} from "react";
import classes from "@/components/widgets/Notes/NotesWidget.module.css";
import {ScrollArea, Table, Text} from "@mantine/core";
import clsx from "clsx";
import dayjs from "dayjs";
import MeetingsGet from "@/lib/widgets/Meetings/MeetingsGet";
import {Meeting} from "@/lib/interfaces/types";
import adjustTimeToLocal from "@/lib/widgets/Meetings/adjustTimeToLocal";
import {useRouter} from "next/navigation";


const MeetingsWidget : React.FC = () => {
    const [scrolled, setScrolled] = useState(false);
    const [meetings, setMeetings] = useState<Meeting[]>([]);

    useEffect(() => {
        (async () => {
            const get = await MeetingsGet();
            setMeetings(get);
        })();
    }, []);

    const router = useRouter();

    function handleClick(meetingId: string) {
        router.push(`/meeting/${meetingId}`);
    }

    const rows = meetings.map((row) => (
        <Table.Tr key={row.meetingId} onClick={() => handleClick(row.meetingId)}>
            <Table.Td>{row.name}</Table.Td>
            <Table.Td>{dayjs(adjustTimeToLocal(row.date,row.timezoneOffset)).format('DD.MM.YYYY HH:mm')}</Table.Td>
            <Table.Td>{row.timezoneName}</Table.Td>
            <Table.Td>{row.place}</Table.Td>
        </Table.Tr>
    ));

    return(
            <div className={classes.allmeetingslist}>
                {
                    rows.length > 0 ?
                        (<ScrollArea h={150} onScrollPositionChange={({ y }) => setScrolled(y !== 0)}>
                            <Table miw={700} stickyHeader>
                                <Table.Thead  className={clsx(classes.header, { [classes.scrolled]: scrolled })}>
                                    <Table.Tr>
                                        <Table.Th>Nazwa spotkania</Table.Th>
                                        <Table.Th>Data rozpoczęcia spotkania</Table.Th>
                                        <Table.Th>Strefa czasowa spotkania</Table.Th>
                                        <Table.Th>Miejsce</Table.Th>
                                    </Table.Tr>
                                </Table.Thead>
                                <Table.Tbody>{rows}</Table.Tbody>
                            </Table>
                        </ScrollArea>
                        )
                        :
                        (<div style={{alignItems: "center", alignContent: "center"}}>
                                <Text>Coś tu pusto</Text>
                            </div>
                        )
                }

            </div>
    )
}

export default MeetingsWidget;
