'use client'
import React from "react";
import {Avatar, AvatarGroup, Button, Flex, Group, Space, Text, Title, Tooltip} from "@mantine/core";
import dayjs from "dayjs";
import {ExtendedMeeting} from "@/lib/interfaces/types";
import EditMeeting from "@/components/dashboard/EditMeeting/EditMeeting";
import {closeAllModals, modals, openModal} from "@mantine/modals";
import SetMeetingTimeByUser from "@/components/dashboard/SetMeetingTimeByUser/SetMeetingTimeByUser";
import adjustTimeToLocal from "@/lib/widgets/Meetings/adjustTimeToLocal";
import axiosInstance from "@/lib/utils/AxiosInstance";

interface MeetingDetailsProps {
    data: ExtendedMeeting;
}

const MeetingDetails: React.FC<MeetingDetailsProps> = ({data}) => {
    const handleOpenModal = () => {
        openModal({
            title: <Title order={2}>Edycja spotkania</Title>,
            size: '70%',
            radius: 10,
            children: (<EditMeeting data={data}/>),
        });
    };

    const handlePlanningModeModal = () => modals.openConfirmModal({
        title: <Title order={2}>Zmiana trybu spotkania</Title>, size: '70%', radius: 10, children: (<Text size="sm">
                Zmiana spowoduje tymczasowe wyłączenie trybu planowania spotkania.
                Aby ponownie go włączyć, ponownie dokonaj zmiany trybu.
            </Text>), labels: {confirm: 'Potwierdź', cancel: 'Anuluj'}, onCancel: () => closeAllModals(),//popraw
        onConfirm: () => {
            axiosInstance.post('/Meetings/changeMode/' + data.id);
            closeAllModals();
        },
    });

    const handleTimeModal = () => {
        openModal({
            title: 'Modyfikacja czasu spotkania',
            size: '70%',
            radius: 10,
            children: (<SetMeetingTimeByUser startDateTime={data.startOfTheMeeting} endDateTime={data.endOfTheMeeting}
                                             intervalsPerHour={4}/>),
        });
    };

    const handleDeleteMeetingModal = () => modals.openConfirmModal({
        title: <Title order={2}>Usunięcie spotkania</Title>,
        size: '70%',
        radius: 10,
        children: (<Text size="sm">
                Aby usunąć kliknij przycisk potwierdź. Operacja jest nieodwracalna.
            </Text>),
        labels: {confirm: 'Potwierdź', cancel: 'Anuluj'},
        onCancel: () => closeAllModals(),
        onConfirm: () => {
            axiosInstance.delete('/Meetings/' + data.id).then(r => r.status);
            closeAllModals();
            window.location.href = "/meetings"
        },
    });

    return (<>
            <Title order={2}>Szczegóły spotkania</Title>
            <Text size="xl" c="dimmed" fw={500}>Opis</Text>
            <Text
                size="lg"
            >{data.desc}</Text>
            <Text size="xl" c="dimmed" fw={500}>Adres</Text>
            {data.lon !== 0 ? (<Tooltip
                    label="Otwórz w OpenStreetMap"
                    transitionProps={{transition: 'fade', duration: 300}}
                >
                    <Text
                        component="a"
                        href={`https://www.openstreetmap.org/search?lat=${data.lon}&lon=${data.lat}#map=19/${data.lon}/${data.lat}`}
                        target="_blank"
                        size="lg"
                    >
                        {data.placeName}
                    </Text>
                </Tooltip>) : (<Text size="lg">
                    {data.placeName}
                </Text>)}
            <Text size="xl" c="dimmed" fw={500}>Data i godzina rozpoczęcia spotkania</Text>
            <Text size="lg">
                {dayjs(adjustTimeToLocal(data.startOfTheMeeting)).format('DD.MM.YYYY HH:mm')}
            </Text>
            <Text size="xl" c="dimmed" fw={500}>Data i godzina zakończenia spotkania</Text>
            <Text size="lg">
                {dayjs(adjustTimeToLocal(data.endOfTheMeeting)).format('DD.MM.YYYY HH:mm')}
            </Text>{!data.isMeetingTimePlanned ? (<Tooltip
                label={"Aby móc wejść do trybu planowania spotkania musi być on włączony przez właściciela spotkania."}>
                <Button onClick={handleTimeModal} variant="light" disabled={!data.isMeetingTimePlanned}>Wejdź do trybu
                    planowania czasu spotkania</Button>
            </Tooltip>) : (
            <Button onClick={handleTimeModal} variant="light" disabled={!data.isMeetingTimePlanned}>Wejdź do trybu
                planowania czasu spotkania</Button>)}

            <Text size="xl" c="dark" pt={10} fw={500}>Goście</Text>
            {data.confirmedInvites.length > 0 ? (<>
                    <Text size="xl" c="dimmed" fw={500}>Zaakceptowane zaproszenia</Text>
                    <Flex wrap="wrap" gap="xs">
                        <AvatarGroup>
                            {data.confirmedInvites.slice(0, 5).map(invite => (
                                <Tooltip key={invite.personId} label={invite.name}
                                         transitionProps={{transition: 'fade', duration: 300}}>
                                    <Avatar src={"/avatars/" + invite.avatar + ".png"} size={50} alt={invite.name}/>
                                </Tooltip>))}
                            {data.confirmedInvites.length > 5 && (<Tooltip
                                    withArrow
                                    label={<>
                                        {data.confirmedInvites.slice(5).map(invite => (
                                            <div key={invite.personId}>{invite.name}</div>))}
                                    </>}
                                >
                                    <Avatar radius="xl">+{data.confirmedInvites.length - 5}</Avatar>
                                </Tooltip>)}
                        </AvatarGroup>
                    </Flex>
                </>) : (<></>)}
            {data.sendInvites.length > 0 ? (<>
                    <Text size="xl" c="dimmed" fw={500}>Wysłane zaproszenia</Text>
                    <Flex wrap="wrap" gap="xs">
                        <AvatarGroup>
                            {data.sendInvites.slice(0, 5).map(invite => (
                                <Tooltip key={invite.personId} label={invite.name}
                                         transitionProps={{transition: 'fade', duration: 300}}>
                                    <Avatar src={"/avatars/" + invite.avatar + ".png"} size={50} alt={invite.name}/>
                                </Tooltip>))}
                            {data.sendInvites.length > 5 && (<Tooltip
                                    withArrow
                                    label={<>
                                        {data.sendInvites.slice(5).map(invite => (
                                            <div key={invite.personId}>{invite.name}</div>))}
                                    </>}
                                >
                                    <Avatar radius="xl">+{data.sendInvites.length - 5}</Avatar>
                                </Tooltip>)}
                        </AvatarGroup>
                    </Flex>
                </>) : (<></>)}
            {data.rejectedInvites.length > 0 ? (<>
                    <Text size="xl" c="dimmed" fw={500}>Odrzucone zaproszenia</Text>
                    <Flex wrap="wrap" gap="xs">
                        <AvatarGroup>
                            {data.rejectedInvites.slice(0, 5).map(invite => (
                                <Tooltip key={invite.personId} label={invite.name}
                                         transitionProps={{transition: 'fade', duration: 300}}>
                                    <Avatar src={"/avatars/" + invite.avatar + ".png"} size={50} alt={invite.name}/>
                                </Tooltip>))}
                            {data.rejectedInvites.length > 5 && (<Tooltip
                                    withArrow
                                    label={<>
                                        {data.rejectedInvites.slice(5).map(invite => (
                                            <div key={invite.personId}>{invite.name}</div>))}
                                    </>}
                                >
                                    <Avatar radius="xl">+{data.rejectedInvites.length - 5}</Avatar>
                                </Tooltip>)}
                        </AvatarGroup>
                    </Flex>
                </>) : (<></>)}
            <Space h={"md"}/>
            <Group>
                {data.isRequestingUserAnOwner ? (<><Button onClick={handlePlanningModeModal}>
                        {data.isMeetingTimePlanned ? "Wyłącz tryb ustalania godziny" : "Włącz tryb ustalania godziny"}
                    </Button>
                        <Button onClick={handleOpenModal}>Zmodyfikuj szczegóły spotkania</Button>
                        <Button color="red" onClick={handleDeleteMeetingModal}>Usuń spotkanie</Button>
                    </>) : (<></>)}
            </Group>
        </>);
};

export default MeetingDetails;
