import React from "react";
import {Avatar, AvatarGroup, Button, Title, Text, rem, Group, GridCol, Grid, Flex} from "@mantine/core";
import classes from "./NextMeetingWidget.module.css";

const NextMeetingWidget : React.FC = () => {

    return(
        <div className={classes.body}>
            <Title order={1}>18 Urodziny Fifonża</Title>
            <Grid>
            <Grid.Col span={7}>
                    <Text size={rem(20)}>24 Lipiec </Text>
                    <Text size={rem(20)}>(za 3 dni)</Text>
                </Grid.Col>
                <Grid.Col span={4}>
                    <Text size={rem(40)}>18:00</Text>
                </Grid.Col>
            </Grid>
            <div>
            <Text>Klubokawiarnia Quest</Text>
            <Text>Duża 21, 25-385 Kielce</Text>
            </div>
            <Flex p={30} justify={"center"} align={"center"} gap={30}>
                <div style={{alignContent: "center", alignSelf: "center"}}>
                    Uczestnicy
                    <AvatarGroup>
                        <Avatar color="red">AR</Avatar>
                        <Avatar color="violet">MD</Avatar>
                        <Avatar color="blue">PK</Avatar>
                        <Avatar color="green">PM</Avatar>
                        <Avatar>+8</Avatar>
                    </AvatarGroup>
                </div>
                <Button variant="outline">Szczegóły</Button>
            </Flex>
        </div>
    )
}

export default NextMeetingWidget;
