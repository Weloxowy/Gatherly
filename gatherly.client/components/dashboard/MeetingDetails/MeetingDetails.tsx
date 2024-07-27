'use client'
import React from "react";
import {Avatar, AvatarGroup, Button, Text, TextInput, Title, Tooltip} from "@mantine/core";
import dayjs from "dayjs";
import {ExtendedMeeting} from "@/lib/meetingDetails/meetingDetails";

interface MeetingDetailsProps {
    data: ExtendedMeeting;
}

const MeetingDetails: React.FC<MeetingDetailsProps> = ({data}) => {

    return (<>
            <Title order={2}>Szczegóły spotkania</Title>
            <Text size="xl" c="dimmed" fw={500}>Opis</Text>
            <TextInput
                variant="unstyled"
                size="md"
                value={data.desc}
            />
            <Text size="xl" c="dimmed" fw={500}>Adres</Text>
            <Tooltip
                label="Otwórz w OpenStreetMap"
                transitionProps={{transition: 'fade', duration: 300}}
            >
                <Text component="a" href="https://maps.app.goo.gl/YrbAmuTYhNEwt45W6" target="_blank" size="lg">
                    {data.placeName}
                </Text>
            </Tooltip>
            <Text size="xl" c="dimmed" fw={500}>Data i godzina spotkania</Text>
            <Text size="lg">
                {dayjs(data.date).format('DD.MM.YYYY HH:mm')}
            </Text>
            <Text size="xl" c="dimmed" fw={500}>Potwierdzone zaproszenia</Text>
            <AvatarGroup>
                {data.confirmedInvites.map(invite => (
                    <Tooltip key={invite.id} label={invite.name} transitionProps={{transition: 'fade', duration: 300}}>
                        <Avatar src={invite.avatar} alt={invite.name}/>
                    </Tooltip>))}
            </AvatarGroup>
            <Text size="xl" c="dimmed" fw={500}>Wysłane zaproszenia</Text>
            <AvatarGroup>
                {data.sendInvites.map(invite => (
                    <Tooltip key={invite.id} label={invite.name} transitionProps={{transition: 'fade', duration: 300}}>
                        <Avatar src={invite.avatar} alt={invite.name}/>
                    </Tooltip>))}
            </AvatarGroup>
            <Text size="xl" c="dimmed" fw={500}>Odrzucone zaproszenia</Text>
            <AvatarGroup>
                {data.rejectedInvites.map(invite => (
                    <Tooltip key={invite.id} label={invite.name} transitionProps={{transition: 'fade', duration: 300}}>
                        <Avatar src={invite.avatar} alt={invite.name}/>
                    </Tooltip>))}
            </AvatarGroup>
            <div style={{alignItems: "stretch", alignContent: "stretch"}}>
                <Button>Zamknij spotkanie</Button>
                <Button variant="outline" color="red">Usuń spotkanie</Button>
            </div>
        </>);
};

export default MeetingDetails;
