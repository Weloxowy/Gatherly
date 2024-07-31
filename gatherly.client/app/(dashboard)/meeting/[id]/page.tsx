'use client';
import React, {useEffect, useState} from 'react';
import {useParams} from 'next/navigation';
import {getExtendedMeetingById} from '@/lib/meetingDetails/meetingDetails';
import {ExtendedMeeting} from '@/lib/interfaces/types';
import classes from "@/app/(dashboard)/meeting/[id]/MeetingComponent.module.css";
import {Container, Grid, Paper, rem, SimpleGrid, Title} from "@mantine/core";
import MeetingDetails from "@/components/dashboard/MeetingDetails/MeetingDetails";
import dynamic from "next/dynamic";
import InviteWidget from "@/components/widgets/Invite/InviteWidget";

const MeetingMap = dynamic(() => import('@/components/widgets/MeetingMap/MeetingMapWidget'), {
    ssr: false
});

export default function Meeting() {
    const {id} = useParams();
    const [data, setData] = useState<ExtendedMeeting | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        if (id) {
            const meetingId = id.toString();

            try {
                const meeting = getExtendedMeetingById(meetingId);
                if (meeting) {
                    setData(meeting);
                } else {
                    setError('Meeting not found');
                }
            } catch (err) {
                setError('Failed to load');
            } finally {
                setLoading(false);
            }
        }
    }, [id]);

    if (loading) return <div>Loading...</div>;
    if (error) return <div>{error}</div>;

    return (<div className={classes.whole}>
        <div className={classes.name}>
            <Title order={2}>{data?.name}</Title>
            <>Spotkanie utworzył: <b>Anna Wiech</b> | 18.03.2024 19:30</>
        </div>
        <Container my="lg">
            <SimpleGrid cols={{base: 1, sm: 2}} spacing="md">
                <Paper radius="lg" shadow="lg" p="lg" style={{height: "100%", width: "100%"}}>
                    {data && <MeetingDetails data={data}/>}
                </Paper>
                <Grid gutter="md" w={rem(1000)}>
                    <Grid.Col span={9}>
                        <Paper radius="lg" shadow="lg" style={{height: "30vh"}}>
                            {data?.lon != null && data.lat != null ? (<MeetingMap lon={data.lon}
                                                                                  lat={data.lat}/>) : ('Brak koordynatów. Tu będzie coś innego')}
                        </Paper>
                    </Grid.Col>
                    <Grid.Col span={3}>
                        <Paper radius="lg" shadow="lg" p="lg" style={{height: "30vh"}}>
                            <InviteWidget/>
                        </Paper>
                    </Grid.Col>
                    <Grid.Col span={12}>
                        <Paper radius="lg" shadow="lg" p="lg" style={{height: "50vh"}}>
                            chat
                        </Paper>
                    </Grid.Col>
                </Grid>
            </SimpleGrid>
        </Container>
    </div>
    );
};
