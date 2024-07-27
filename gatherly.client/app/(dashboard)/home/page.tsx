"use client"
import React from "react";
import {Container, Grid, Paper, SimpleGrid, Title} from "@mantine/core";
import classes from './Home.module.css';
import CalendarCardWidget from "@/components/widgets/CalendarCard/CalendarCardWidget";
import NotesWidget from "@/components/widgets/Notes/NotesWidget";
import NextMeetingWidget from "@/components/widgets/NextMeeting/NextMeetingWidget";
import clsx from "clsx";
import MeetingsWidget from "@/components/widgets/Meetings/MeetingsWidget";
import {BsCalendar2Event, BsListCheck} from "react-icons/bs";

export default function Home() {
    return (<div className={classes.main}>
            <div className={classes.name}>
                <Title order={2}>Witaj, Anno</Title>
                Czas na zorganizowanie spotkania!
            </div>
            <Container my="lg">
                <SimpleGrid cols={{base: 1, sm: 2}} spacing="md">
                    <Paper radius="lg" shadow="lg" p="lg" className={classes.componentMd}>
                        <span className={classes.title}>
                            <span className={classes.icon}>
                                <BsCalendar2Event color="white"/>
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
                                        <BsListCheck color="white"/>
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
                                <Title>Gatherly</Title>
                            </Paper>
                        </Grid.Col>
                    </Grid>
                    <Paper radius="lg" shadow="lg" p="lg" className={classes.componentXl}>
                        <div className={classes.title}>
                            <span className={classes.icon}>
                                <BsListCheck color="white"/>
                            </span>
                            Najbliższe spotkania
                        </div>
                        <MeetingsWidget/>
                    </Paper>
                </SimpleGrid>
            </Container>
        </div>);
}
