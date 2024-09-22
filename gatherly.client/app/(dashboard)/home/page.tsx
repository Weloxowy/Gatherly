"use client"
import React, {useEffect, useState} from "react";
import {Button, Container, Grid, Group, Paper, SimpleGrid, Title} from "@mantine/core";
import classes from './Home.module.css';
import CalendarCardWidget from "@/components/widgets/CalendarCard/CalendarCardWidget";
import NotesWidget from "@/components/widgets/Notes/NotesWidget";
import NextMeetingWidget from "@/components/widgets/NextMeeting/NextMeetingWidget";
import clsx from "clsx";
import MeetingsWidget from "@/components/widgets/Meetings/MeetingsWidget";
import {IconCalendarPlus, IconListCheck, IconCalendarEvent} from "@tabler/icons-react";
import {openModal} from "@mantine/modals";
import NewMeeting from "@/components/dashboard/NewMeeting/NewMeeting";
import JwtTokenValid from "@/lib/auth/GetUserInfo";

export default function Home() {
    const [name, setName] = useState('');

    useEffect(() => {
        const fetchUserInfo = async () => {
            const x = await JwtTokenValid();
            if (x) {
                setName(x.name);
            }
        };
        fetchUserInfo();
    }, []);

    const handleOpenModal = () => {
        openModal({
            title: <Title order={2}>Nowe spotkanie</Title>,
            size: '70%',
            radius: 10,
            children: (
                <NewMeeting/>
            ),
        });
    };

    return (<div className={classes.main}>
        <Group className={classes.head}>
            <div className={classes.name}>
                <Title order={2}>Witaj, {name}</Title>
                Czas na zorganizowanie spotkania!
            </div>
            <Button className={classes.buttonHead} variant={"outline"} rightSection={<IconCalendarPlus size={14}/>}
                    onClick={() => handleOpenModal()}>Utwórz spotkanie </Button>
        </Group>
        <Container my="lg" m={0}>
            <SimpleGrid cols={{base: 1, lg: 2}} w={{base: "100%", sm: "100%"}} spacing="md">
                <Paper radius="lg" shadow="lg" p="lg" className={classes.componentMd}>
                        <span className={classes.title}>
                            <span className={classes.icon}>
                                <IconCalendarEvent color="white"/>
                            </span>
                            Następne spotkanie
                        </span>
                    <NextMeetingWidget/>
                </Paper>
                <Grid gutter="md">
                    <Grid.Col>
                        <Paper radius="lg" shadow="lg" p="lg" className={classes.componentLg}>
                            <div className={classes.title}>
                                    <span className={classes.icon}>
                                        <IconListCheck color="white"/>
                                    </span>
                                Przypomnienia
                            </div>
                            <NotesWidget/>
                        </Paper>
                    </Grid.Col>
                    <Grid.Col span={6}>
                        <Paper radius="lg" shadow="lg" p="lg" className={classes.componentRect}>
                            <CalendarCardWidget/>
                        </Paper>
                    </Grid.Col>
                    <Grid.Col span={6}>
                        <Paper radius="lg" shadow="lg" p="lg"
                               className={clsx(classes.componentSm, classes.component)}>
                            <div className={classes.blurCircle}></div>
                            <div className={classes.blurCircle}></div>
                            <div className={classes.blurCircle}></div>
                            <div className={classes.blurCircle}></div>
                            <Title className={classes.responsiveTitle}>Gatherly</Title>
                        </Paper>
                    </Grid.Col>
                </Grid>
                <Paper radius="lg" shadow="lg" p="lg" className={classes.componentXl}>
                    <div className={classes.title}>
                            <span className={classes.icon}>
                                <IconListCheck color="white"/>
                            </span>
                        Najbliższe spotkania
                    </div>
                    <MeetingsWidget/>
                </Paper>
            </SimpleGrid>
        </Container>
    </div>);
}
