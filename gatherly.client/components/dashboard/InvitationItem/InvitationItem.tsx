import {InvitationMeeting} from "@/lib/interfaces/types";
import {Button, Group, Paper, Title} from "@mantine/core";
import React from "react";
import dayjs from "dayjs";
import axiosInstance from "@/lib/utils/AxiosInstance";
import {closeAllModals} from "@mantine/modals";
import {notifications} from "@mantine/notifications";
import {useGlobalNotifications} from "@/components/notifications/NotificationContext";

const InvitationItem = ({data}: { data: InvitationMeeting }) => {
    const { showNotification } = useGlobalNotifications();
    function handleAccept() {
        axiosInstance.post('Invitations/' + data.InvitationId + '/confirm').then(r => {
            notifications.show({
                loading: true,
                title: 'Spotkanie zostało potwierdzone',
                message: 'Zostaniesz zaraz do niego przeniesiony, nie zamykaj aplikacji',
                autoClose: false,
                withCloseButton: false,
            });
            window.location.href = window.location.origin + "/meeting/" + data.meetingId;
        });
        closeAllModals();
    }

    function handleDecline() {
            showNotification({
                title: 'Spotkanie zostało odrzucone',
                message: 'super',
                type: 'success', // można ustawić 'error', 'info' itp.
            });

        axiosInstance.delete('Invitations/' + data.InvitationId + '/decline').then(r => {
           /*
            notifications.show({
                loading: false,
                title: 'Spotkanie zostało odrzucone',
                message: 't',
                autoClose: false,
                withCloseButton: false,
            });
            */
            window.location.reload()
        });
        closeAllModals();
    }

    return (<Paper radius="lg" shadow="lg" p="lg" m="lg">
            <div style={{marginLeft: "1rem"}}>
                <Title order={2}>{data.MeetingName}</Title>
                <Group style={{alignContent: "stretch", alignItems: "center"}}>
                    <>
                        {dayjs(data.StartOfTheMeeting).format("DD.MM.YYYY HH:mm")} | {dayjs(data.EndOfTheMeeting).format("DD.MM.YYYY HH:mm")}
                    </>
                    <Button variant={"filled"} onClick={handleAccept}>Przyjmij zaproszenie</Button>
                    <Button variant={"filled"} color={"red"} onClick={handleDecline}>Odrzuć zaproszenie</Button>
                </Group>
            </div>
        </Paper>)
}

export default InvitationItem;
