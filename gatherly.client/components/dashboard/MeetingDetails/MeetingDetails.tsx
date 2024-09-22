'use client'
import React, {useEffect, useState} from "react";
import {Avatar, AvatarGroup, Button, Fieldset, Flex, ScrollArea, Space, Text, Title, Tooltip} from "@mantine/core";
import dayjs from "dayjs";
import {ExtendedMeeting} from "@/lib/interfaces/types";
import EditMeeting from "@/components/dashboard/MeetingModals/EditMeeting";
import {openModal} from "@mantine/modals";
import SetMeetingTimeByUser from "@/components/dashboard/SetMeetingTimeByUser/SetMeetingTimeByUser";
import adjustTimeToLocal from "@/lib/widgets/Meetings/adjustTimeToLocal";
import axiosInstance from "@/lib/utils/AxiosInstance";
import EditStatus from "@/components/dashboard/MeetingModals/EditStatus";
import EditPlanningMode from "@/components/dashboard/MeetingModals/EditPlanningMode";
import DeleteMeeting from "@/components/dashboard/MeetingModals/DeleteMeeting";
import LeaveMeeting from "@/components/dashboard/MeetingModals/LeaveMeeting";
import {addNotification} from "@/lib/utils/notificationsManager";
import SetMeetingTimeByAdmin from "@/components/dashboard/SetMeetingTimeByAdmin/SetMeetingTimeByAdmin";

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
                //console.error('Error fetching meeting status:', error);
                addNotification({
                    title: 'Wystąpił błąd',
                    message: 'Informacje o spotkaniu nie zostały pobrane pomyślnie.',
                    color: 'red',
                });
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

    const handleStatusModal = (status: number) => {
        openModal({
            title: <Title order={2}>Zmiana statusu</Title>,
            size: '70%',
            radius: 10,
            children: (<EditStatus data={data} status={status} onSubmit={() => {
                setUserMeetingStatus(status)
                window.location.reload();
            }}/>),
        });
    };

    const handlePlanningModeModal = () => {
        openModal({
            title: <Title order={2}>Zmiana statusu</Title>,
            size: '70%',
            radius: 10,
            children: (<EditPlanningMode id={data.id} onSubmit={() => {
                window.location.reload();
            }}/>),
        });
    };

    const handleTimeModal = () => {
        openModal({
            title: <Title order={2}>Modyfikacja czasu spotkania</Title>,
            size: '70%',
            radius: 10,
            children: (data.isRequestingUserAnOwner ? (
                <SetMeetingTimeByAdmin startDateTime={data.startOfTheMeeting} endDateTime={data.endOfTheMeeting}
                                       intervalsPerHour={1}/>) : (
                <SetMeetingTimeByUser startDateTime={data.startOfTheMeeting} endDateTime={data.endOfTheMeeting}
                                      intervalsPerHour={1}/>)),
        });
    };

    const handleDeleteMeetingModal = () => {
        openModal({
            title: <Title order={2}>Usunięcie spotkania</Title>,
            size: '70%',
            radius: 10,
            children: (<DeleteMeeting id={data.id} onSubmit={() => {
                window.location.href = "/meetings"
            }}/>),
        });
    };

    const handleLeaveMeetingModal = () => {
        openModal({
            title: <Title order={2}>Wyjście ze spotkania</Title>,
            size: '70%',
            radius: 10,
            children: (<LeaveMeeting id={data.id} onSubmit={() => {
                window.location.href = "/meetings"
            }}/>),
        });
    };


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
            {userMeetingStatus ? (<Fieldset legend="Zmień status swojej obecności" radius="md" style={{
                display: "flex", gap: "10px", justifyContent: "space-between", flexWrap: "wrap"
            }}>
                <Button disabled={userMeetingStatus === 1} onClick={() => handleStatusModal(1)}>Będę!</Button>
                <Button disabled={userMeetingStatus === 2} onClick={() => handleStatusModal(2)}>Może się
                    zjawię</Button>
                <Button disabled={userMeetingStatus === 3} onClick={() => handleStatusModal(3)}>Nie przyjdę</Button>
            </Fieldset>) : ('')}


            {data.isRequestingUserAnOwner ? (<Fieldset legend="Ustawienia spotkania" radius="md" style={{
                display: "flex", gap: "10px", justifyContent: "space-between", flexWrap: "wrap"
            }}>
                <Button onClick={handlePlanningModeModal}>
                    {data.isMeetingTimePlanned ? "Wyłącz tryb ustalania godziny" : "Włącz tryb ustalania godziny"}
                </Button>
                <Button onClick={handleOpenModal}>Zmodyfikuj szczegóły spotkania</Button>
                <Button color="red" onClick={handleDeleteMeetingModal}>Usuń spotkanie</Button>
            </Fieldset>) : (
                <div style={{display: "flex", flexGrow: 1, alignContent: "center", justifyContent: "stretch"}}>
                    <Button color="red" onClick={handleLeaveMeetingModal}>Wyjdź ze spotkania</Button></div>)}
        </div>
    </ScrollArea>);
};

export default MeetingDetails;
