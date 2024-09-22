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

const DeleteMeeting: React.FC<Props> = ({id, onSubmit}) => {
    const [loading, setLoading] = useState(false);

    const handleSubmit = async () => {
        setLoading(true);
        try {
            await axiosInstance.delete('/Meetings/' + id);
            onSubmit();
            addNotification({
                title: 'Sukces',
                message: 'Spotkanie zostało usunięte pomyślnie.',
                color: 'green',
            });
        } catch (error) {
            console.error("Error while patching meeting:", error);
            addNotification({
                title: 'Wystąpił błąd',
                message: 'Spotkanie nie zostało usunięte pomyślnie.',
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
                Aby usunąć kliknij przycisk potwierdź. Operacja jest nieodwracalna.
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

export default DeleteMeeting;
