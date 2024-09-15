'use client'
import React, {useEffect, useState} from "react";
import {Avatar, AvatarGroup, Button, Fieldset, Flex, ScrollArea, Space, Text, Title, Tooltip} from "@mantine/core";
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
    const [userMeetingStatus, setUserMeetingStatus] = useState<number>(0);

    useEffect(() => {
        const fetchMeetingStatus = async () => {
            try {
                const response = await axiosInstance.get(`/Meetings/meeting/getStatus?meetingId=${data.id}`);
                console.log(response);
                setUserMeetingStatus(response.data);
            } catch (error) {
                console.error('Error fetching meeting status:', error);
                setUserMeetingStatus(-1);
            }
        };
        fetchMeetingStatus();
    }, [data.id]);

    const handleOpenModal = () => {
        openModal({
            title: <Title order={2}>Edycja spotkania</Title>,
            size: '70%',
            radius: 10,
            children: (<EditMeeting data={data}/>),
        });
    };

    const handleStatusModal = (status: number) => modals.openConfirmModal({
        title: <Title order={2}>Zmiana statusu przybycia</Title>, size: '70%', radius: 10, children: (<Text size="sm">
            Potwierdzenie spowoduje zmianę twojego statusu przybycia.
        </Text>), labels: {confirm: 'Potwierdź', cancel: 'Anuluj'}, onCancel: () => closeAllModals(),//popraw
        onConfirm: () => {
            axiosInstance.get('/Meetings/meeting/setStatus?meetingId=' + data.id + '&invitationStatus=' + status);
            setUserMeetingStatus(status);
            closeAllModals();
        },
    });

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
            children: (data.isRequestingUserAnOwner ?
                (<SetMeetingTimeByUser startDateTime={data.startOfTheMeeting} endDateTime={data.endOfTheMeeting} intervalsPerHour={1}/>):
                    ('tu by admin')
            ),
        });
    };

    const handleDeleteMeetingModal = () => modals.openConfirmModal({
        title: <Title order={2}>Usunięcie spotkania</Title>, size: '70%', radius: 10, children: (<Text size="sm">
            Aby usunąć kliknij przycisk potwierdź. Operacja jest nieodwracalna.
        </Text>), labels: {confirm: 'Potwierdź', cancel: 'Anuluj'}, onCancel: () => closeAllModals(), onConfirm: () => {
            axiosInstance.delete('/Meetings/' + data.id).then(r => r.status);
            closeAllModals();
            window.location.href = "/meetings"
        },
    });

    return (<ScrollArea offsetScrollbars style={{flex: 1, overflow: 'auto'}}>
        <div>
            <Title order={2} pb={10}>Szczegóły spotkania</Title>
            <Text size="md" c="dimmed" fw={500}>Opis</Text>
            <Text size="lg" pb={10}>{data.desc}</Text>
            <Text size="md" c="dimmed" fw={500}>Adres</Text>
            {data.lon !== 0 ? (<Tooltip
                label="Otwórz w OpenStreetMap"
                transitionProps={{transition: 'fade', duration: 300}}
            >
                <Text
                    component="a"
                    href={`https://www.openstreetmap.org/search?lat=${data.lon}&lon=${data.lat}#map=19/${data.lon}/${data.lat}`}
                    target="_blank"
                    size="lg"
                    pb={10}
                >
                    {data.placeName}
                </Text>
            </Tooltip>) : (<Text size="lg" pb={10}>
                {data.placeName}
            </Text>)}
            <Text size="md" c="dimmed" fw={500}>Data i godzina rozpoczęcia spotkania</Text>
            <Text size="lg">
                {dayjs(adjustTimeToLocal(data.startOfTheMeeting, data.timezoneOffset)).format('DD.MM.YYYY HH:mm')}
            </Text>
            <Text size="sm" c="gray" pb={10}>
                {dayjs(data.startOfTheMeeting).format('DD.MM.YYYY HH:mm UTC')}
            </Text>
            <Text size="md" c="dimmed" fw={500}>Data i godzina zakończenia spotkania</Text>
            <Text size="lg">
                {dayjs(adjustTimeToLocal(data.endOfTheMeeting, data.timezoneOffset)).format('DD.MM.YYYY HH:mm')}
            </Text>
            <Text size="sm" c="gray" pb={10}>
                {dayjs(data.endOfTheMeeting).format('DD.MM.YYYY HH:mm UTC')}
            </Text>
            <Text size="md" c="dimmed" fw={500}>Strefa czasowa</Text>
            <Text size="lg" pb={10}>
                {data.timezoneName}
            </Text>
            {!data.isMeetingTimePlanned ? (<Tooltip
                label={"Aby móc wejść do trybu planowania spotkania musi być on włączony przez właściciela spotkania."}>
                <Button onClick={handleTimeModal} variant="light" disabled={!data.isMeetingTimePlanned}>Wejdź do trybu
                    planowania czasu spotkania</Button>
            </Tooltip>) : (
                <Button onClick={handleTimeModal} variant="light" disabled={!data.isMeetingTimePlanned}>Wejdź do trybu
                    planowania czasu spotkania</Button>)}

            <Text size="md" c="dark" pt={20} fw={500}>Goście</Text>
            {data.confirmedInvites.length > 0 ? (<>
                <Text size="md" c="dimmed" fw={500}>Zaakceptowane zaproszenia</Text>
                <Flex wrap="wrap" gap="xs" pb={10}>
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
                <Text size="md" c="dimmed" fw={500}>Wysłane zaproszenia</Text>
                <Flex wrap="wrap" gap="xs" pb={10}>
                    <AvatarGroup>
                        {data.sendInvites.slice(0, 5).map(invite => (<Tooltip key={invite.personId} label={invite.name}
                                                                              transitionProps={{
                                                                                  transition: 'fade', duration: 300
                                                                              }}>
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
                <Text size="md" c="dimmed" fw={500}>Odrzucone zaproszenia</Text>
                <Flex wrap="wrap" gap="xs" pb={10}>
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
            {userMeetingStatus ? (
                <Fieldset legend="Zmień status swojej obecności" radius="md" style={{display: "flex", gap: "10px",justifyContent: "space-between", flexWrap: "wrap"}}>
                    <Button disabled={userMeetingStatus===1} onClick={() => handleStatusModal(1)}>Będę!</Button>
                    <Button disabled={userMeetingStatus===2} onClick={() => handleStatusModal(2)}>Może się zjawię</Button>
                    <Button disabled={userMeetingStatus===3} onClick={() => handleStatusModal(3)}>Nie przyjdę</Button>
                </Fieldset>
            ) : ('')}


            {data.isRequestingUserAnOwner ? (<Fieldset legend="Ustawienia spotkania" radius="md" style={{display: "flex", gap: "10px",justifyContent: "space-between", flexWrap: "wrap"}}>
                <Button onClick={handlePlanningModeModal}>
                    {data.isMeetingTimePlanned ? "Wyłącz tryb ustalania godziny" : "Włącz tryb ustalania godziny"}
                </Button>
                <Button onClick={handleOpenModal}>Zmodyfikuj szczegóły spotkania</Button>
                <Button color="red" onClick={handleDeleteMeetingModal}>Usuń spotkanie</Button>
            </Fieldset>) : (<></>)}
        </div>
    </ScrollArea>);
};

export default MeetingDetails;
