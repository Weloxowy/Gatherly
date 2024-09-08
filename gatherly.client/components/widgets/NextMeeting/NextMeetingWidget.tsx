import React, { useEffect, useState } from "react";
import { Avatar, AvatarGroup, Button, Flex, Grid, rem, Skeleton, Text, Title, Tooltip } from "@mantine/core";
import classes from "./NextMeetingWidget.module.css";
import { Meeting, Person } from "@/lib/interfaces/types";
import NextMeeting from "@/lib/widgets/NextMeeting/NextMeeting";
import dayjs from "dayjs";
import 'dayjs/locale/pl';
dayjs.locale('pl');
import GetNextMeetingUsers from "@/lib/widgets/NextMeeting/GetNextMeetingUsers";
import Link from "next/link";
import adjustTimeToLocal from "@/lib/widgets/Meetings/adjustTimeToLocal";
import { openModal } from "@mantine/modals";
import NewMeeting from "@/components/dashboard/NewMeeting/NewMeeting";
import { IconCalendarPlus } from "@tabler/icons-react";

const NextMeetingWidget: React.FC = () => {
    const [meeting, setMeeting] = useState<Meeting | null>(null); // Zmieniony typ na Meeting | null
    const [additionalUsers, setAdditionalUsers] = useState<number>(0);
    const [meetingId, setMeetingId] = useState<string | null>(null);
    const [users, setUsers] = useState<Person[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);

    const handleOpenModal = () => {
        openModal({
            title: 'Nowe spotkanie',
            size: '70%',
            radius: 10,
            children: (
                <NewMeeting />
            ),
        });
    };

    useEffect(() => {
        (async () => {
            const get = await NextMeeting();
            if (get) {
                setMeeting(get);
                setMeetingId(get.meetingId); // Poprawne przypisanie ID
            }
            setIsLoading(false);
        })();
    }, []);

    useEffect(() => {
        (async () => {
            if (meetingId && meeting) { // Dodano sprawdzenie, czy meetingId i meeting są ustawione
                const get = await GetNextMeetingUsers(meetingId);
                setAdditionalUsers(get.length - 5);
                setUsers(get.slice(0, 5));
            }
        })();
    }, [meetingId, meeting]); // Dodano meeting jako zależność

    if (isLoading) {
        return (
            <>
                <Skeleton height={8} radius="xl" />
                <Skeleton height={8} mt={6} radius="xl" />
                <Skeleton height={8} mt={6} width="70%" radius="xl" />
            </>
        );
    }

    return (
        <>
            {meeting === null ? ( // Zmieniono warunek sprawdzający na meeting === null
                <>
                    Ooh! Nie ma żadnego spotkania.
                    <Button className={classes.buttonHead} variant={"outline"} rightSection={<IconCalendarPlus size={14} />}
                            onClick={() => handleOpenModal()}>Utwórz spotkanie </Button>
                </>
            ) : (
                <div key={meeting.meetingId} className={classes.body}>
                    <Title order={1}>{meeting.name}</Title>
                    <Grid>
                        <Grid.Col span={7}>
                            <Text size={rem(20)}>{dayjs(adjustTimeToLocal(meeting.date)).format('D MMMM')}</Text>
                            <Text size={rem(20)}>
                                (za {dayjs(adjustTimeToLocal(meeting.date)).diff(dayjs(), 'day')} dni)
                            </Text>
                        </Grid.Col>
                        <Grid.Col span={4}>
                            <Text size={rem(40)}>{dayjs(adjustTimeToLocal(meeting.date)).format('HH:mm')}</Text>
                        </Grid.Col>
                    </Grid>
                    <div>
                        <Text>{meeting.place}</Text>
                    </div>
                    <Flex p={30} justify={"center"} align={"center"} gap={30}>
                        <div style={{ alignContent: "center", alignSelf: "center" }}>
                            Uczestnicy
                            <AvatarGroup>
                                {users.map((user) => (
                                    <div key={user.personId}>
                                        <Tooltip key={user.personId} label={user.name} transitionProps={{ transition: 'fade', duration: 300 }}>
                                            <Avatar src={"/avatars/" + user.avatar + ".png"}></Avatar>
                                        </Tooltip>
                                    </div>
                                ))}
                                {additionalUsers > 0 ? (
                                    <Avatar>+{additionalUsers}</Avatar>
                                ) : ('')}
                            </AvatarGroup>
                        </div>
                        <Button variant="outline" component={Link} href={"/meeting/" + meeting.meetingId}>Szczegóły</Button>
                    </Flex>
                </div>
            )}
        </>
    );
};

export default NextMeetingWidget;
