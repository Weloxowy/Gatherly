import {InvitationMeeting} from "@/lib/interfaces/types";
import {Button, Group, Paper, Title} from "@mantine/core";
import React from "react";
import dayjs from "dayjs";
import axiosInstance from "@/lib/utils/AxiosInstance";
import {closeAllModals} from "@mantine/modals";

const InvitationItem = ({data}: { data: InvitationMeeting }) => {
    function handleAccept() {
        axiosInstance.post('Invitations/' + data.InvitationId + '/confirm').then(r => {
            window.location.href = "https://localhost:3000/meeting/" + data.meetingId
        });
        closeAllModals();
    }

    function handleDecline() {
        axiosInstance.delete('Invitations/' + data.InvitationId + '/decline').then(r => {
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
