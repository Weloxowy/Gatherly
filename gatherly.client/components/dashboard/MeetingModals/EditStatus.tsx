'use client';
import React, {useState} from "react";
import {Button, Group, Text} from "@mantine/core";
import {ExtendedMeeting} from "@/lib/interfaces/types";
import 'dayjs/locale/pl';
import {closeAllModals} from "@mantine/modals";
import axiosInstance from "@/lib/utils/AxiosInstance";
import {addNotification} from "@/lib/utils/notificationsManager";

interface Props {
    data: ExtendedMeeting;
    status: number;
    onSubmit: () => void;
}

const EditStatus: React.FC<Props> = ({data, status, onSubmit}) => {
    const [loading, setLoading] = useState(false);

    const handleSubmit = async () => {
        setLoading(true);
        try {
            setLoading(true);
            await axiosInstance.put('/Meetings/meeting/setStatus?meetingId=' + data.id + '&invitationStatus=' + status);
            onSubmit();
            setLoading(false);
            addNotification({
                title: 'Sukces',
                message: 'Zmiany zostały zapisane pomyślnie.',
                color: 'green',
            });
            closeAllModals();
        } catch (error) {
            addNotification({
                title: 'Wystąpił błąd',
                message: 'Zmiany nie zostały zapisane pomyślnie.',
                color: 'red',
            });
            //console.error("Error while patching meeting:", error);
        } finally {
            setLoading(false);
            closeAllModals();
        }
    };

    const handleRejectChanges = () => {
        closeAllModals();
    };

    return (<>
            <Text size="sm">
                Potwierdzenie spowoduje zmianę twojego statusu przybycia.
            </Text>
            <Group style={{
                alignItems: "flex-start", alignContent: "center", justifyContent: "space-between", paddingTop: "20px"
            }}>
                <Group ml="0">
                    <Button loading={loading} onClick={handleSubmit}>Zatwierdź zmiany</Button>
                    <Button loading={loading} variant="outline" color="red" onClick={handleRejectChanges}>Odrzuć
                        zmiany</Button>
                </Group>
            </Group>
        </>);
}

export default EditStatus;
