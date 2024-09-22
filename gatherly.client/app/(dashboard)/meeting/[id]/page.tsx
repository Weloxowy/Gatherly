'use client';
import React, {useEffect, useState} from 'react';
import {useParams} from 'next/navigation';
import {ExtendedMeeting} from '@/lib/interfaces/types';
import classes from "@/app/(dashboard)/meeting/[id]/MeetingComponent.module.css";
import {Button, Container, Grid, LoadingOverlay, Modal, Paper, rem, SimpleGrid, Skeleton, Title} from "@mantine/core";
import MeetingDetails from "@/components/dashboard/MeetingDetails/MeetingDetails";
import dynamic from "next/dynamic";
import InviteIcon from "@/components/widgets/Invite/InviteIcon";
import '@mantine/dates/styles.css';
import GetMeeting from "@/lib/widgets/GetMeeting";
import {useDisclosure} from "@mantine/hooks";
import dayjs from "dayjs";
import ChatWidget from "@/components/widgets/Chat/ChatWidget";
import {openModal} from "@mantine/modals";
import InviteWidget from "@/components/widgets/Invite/InviteWidget";
import adjustTimeToLocal from "@/lib/widgets/Meetings/adjustTimeToLocal";
import {addNotification} from "@/lib/utils/notificationsManager";

const MeetingMap = dynamic(() => import('@/components/widgets/MeetingMap/MeetingMapWidget'), {
    ssr: false
});

export default function Meeting() {
    const {id} = useParams();
    const [data, setData] = useState<ExtendedMeeting | null>(null);
    const [error, setError] = useState(false);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        const fetchMeeting = async () => {
            if (id) {
                const meetingId = id.toString();

                try {
                    setLoading(true);
                    const meeting = await GetMeeting(meetingId);

                    if (meeting) {
                        setData(meeting);
                    } else {
                        addNotification({
                            title: 'Wystąpił błąd',
                            message: 'Spotkanie nie zostało znalezione.',
                            color: 'red',
                        });
                        setError(true);
                    }
                } catch (err) {
                    setError(true);
                } finally {
                    setLoading(false);
                }
            }
        };

        fetchMeeting();
    }, [id]);

    if (loading) return <div>
        <LoadingOverlay
            visible={loading}
            zIndex={1000}
            overlayProps={{ radius: 'sm', blur: 2 }}
            loaderProps={{ color: 'violet', type: 'bars' }}
        />
    </div>;

    if (error) return <div style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        height: '100%',
        minHeight: '100%',
    }}>
        Spotkanie nie zostało znalezione. Spróbuj ponownie.
    </div>;

    const handleOpenModal = () => {
        openModal({
            title: <Title order={3}>Zaproś gości</Title>,
            size: '70%',
            radius: 10,
            children: (
                //@ts-ignore
                <InviteWidget data={data}/>
            ),
        });
    };

    return (<div className={classes.whole}>
            <div className={classes.name}>
                <Title order={2}>{data?.name}</Title>

                <>Spotkanie utworzył(a): <b>{data?.ownerName}</b> | {dayjs(//@ts-ignore
                    data?.creationTime).format("DD.MM.YYYY")}</>
            </div>
        <Container my="lg" m={0}>
            <SimpleGrid cols={{base: 1, sm: 2}} spacing="md">
                <Paper radius="lg" shadow="lg" p="lg" style={{height: "82vh", width: "100%", display: 'flex', flexDirection: 'column'}}>
                    {data && <MeetingDetails data={data}/>}
                </Paper>
                <Grid gutter="md" w={rem(1000)}>
                    <Grid.Col span={9}>
                        <Paper radius="lg" shadow="lg" style={{height: "30vh", zIndex: 1}}>
                            {data?.lon != 0 && //@ts-ignore
                            data.lat != 0 ? (<MeetingMap lon={data.lon} lat={data.lat}/>) : ('')}
                        </Paper>
                    </Grid.Col>
                    <Grid.Col span={3}>
                        <Paper radius="lg" shadow="lg" p="lg" style={{height: "30vh"}} onClick={data?.isRequestingUserAnOwner === true ? handleOpenModal : undefined}>
                            <InviteIcon isAdmin={data?.isRequestingUserAnOwner === true} />
                        </Paper>
                    </Grid.Col>
                    <Grid.Col span={12}>
                        <Paper radius="lg" shadow="lg" p="lg" style={{height: "50vh"}}>
                            {//@ts-ignore
                                data && <ChatWidget
                                    meetingId={data?.id}
                                    usersList={[
                                        ...(data?.confirmedInvites ?? []),
                                        ...(data?.rejectedInvites ?? []),
                                        ...(data?.sendInvites ?? [])
                                    ]}
                                />
                            }
                        </Paper>
                    </Grid.Col>
                </Grid>
            </SimpleGrid>
        </Container>
    </div>
    );
};
