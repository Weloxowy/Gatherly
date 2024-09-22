'use client';
import React, {useState} from "react";
import {Button, Group, Text} from "@mantine/core";
import {ExtendedMeeting} from "@/lib/interfaces/types";
import 'dayjs/locale/pl';
import {closeAllModals} from "@mantine/modals";
import axiosInstance from "@/lib/utils/AxiosInstance";
import {addNotification} from "@/lib/utils/notificationsManager";

interface Props {
    id: string;
    onSubmit: () => void;
}

const EditPlanningMode: React.FC<Props> = ({id, onSubmit}) => {
    const [loading, setLoading] = useState(false);

    const handleSubmit = async () => {
        setLoading(true);
        try {
            setLoading(true);
            await axiosInstance.post('/Meetings/changeMode/' + id).then(onSubmit)
            addNotification({
                title: 'Sukces',
                message: 'Zmiany zostały zapisane pomyślnie.',
                color: 'green',
            });
        } catch (error) {
            //console.error("Error while changing planning mode:", error);
            addNotification({
                title: 'Wystąpił błąd',
                message: 'Zmiany nie zostały zapisane pomyślnie.',
                color: 'red',
            });
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
            Zmiana spowoduje tymczasowe wyłączenie trybu planowania spotkania.
            Aby ponownie go włączyć, ponownie dokonaj zmiany trybu.
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

export default EditPlanningMode;
