"use client";
import {ActionIcon, Avatar, AvatarGroup, Button, Flex, Text, TextInput, Title, Tooltip} from "@mantine/core";
import { ExtendedMeeting, Person } from "@/lib/interfaces/types";
import React, { useEffect, useState } from "react";
import { useForm } from "@mantine/form";
import CreateInvite from "@/lib/widgets/Meetings/CreateInvite";
import GetInvitesForMeeting from "@/lib/widgets/Meetings/GetInvitesForMeeting";
import GetPendingForMeeting from "@/lib/widgets/Meetings/GetPendingForMeeting";
import { IconUserMinus } from "@tabler/icons-react";
import DeleteInvitation from "@/lib/widgets/Meetings/DeleteInvitation";
import DeleteUserMeeting from "@/lib/widgets/Meetings/DeleteUserMeeting";
import {closeAllModals, modals} from "@mantine/modals";

interface MeetingDetailsProps {
    data: ExtendedMeeting;
}

const InviteWidget: React.FC<MeetingDetailsProps> = ({ data }) => {
    const form = useForm({
        initialValues: {
            email: '',
            meetingId: data.id,
        },
        validate: {
            email: (value) => (/^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/.test(value) ? null : 'Nieprawidłowy adres email'),
        },
    });

    const handleOpenModal = (userName: string) => {
        return new Promise((resolve) => {
            modals.openConfirmModal({
                title: <Title order={3}>Potwierdź usunięcie</Title>,
                size: '70%',
                radius: 10,
                children: (
                    <Text size={"sm"}>
                        Potwiedź usunięcie użytkownika {userName} ze spotkania. Operacja jest nieodwracalna. <br />
                        Aby ponownie użytkownik dołączył do spotkania, będzie konieczne ponowne wystosowanie zaproszenia.
                    </Text>
                ),
                labels: { confirm: 'Potwierdź', cancel: 'Anuluj' },
                onCancel: () => {
                    closeAllModals();
                    resolve(false);  // Return false to indicate cancellation
                },
                onConfirm: () => {
                    closeAllModals();
                    resolve(true);  // Return true to indicate confirmation
                },
            });
        });
    };

    const [invitedUsers, setInvitedUsers] = useState<Person[]>([]);
    const [pendingUsers, setPendingUsers] = useState<Person[]>([]);
    const [refreshData, setRefreshData] = useState(false); // Stan odpowiedzialny za odświeżanie danych

    useEffect(() => {
        (async () => {
            try {
                const get = await GetInvitesForMeeting(data.id);
                setInvitedUsers(get);
                console.log(get);
                setRefreshData(false);
            } catch (error) {
                console.error("Failed to fetch invited users", error);
            }
        })();
    }, [data.id,refreshData]);

    useEffect(() => {
        (async () => {
            try {
                const get = await GetPendingForMeeting(data.id);
                console.log(get);
                setPendingUsers(get);
                setRefreshData(false);
            } catch (error) {
                console.error("Failed to fetch users with pending invitation", error);
            }
        })();
    }, [data.id,refreshData]);

    const handleInviteForm = async (values: { email: string, meetingId: string }) => {
        try {
            const response = await CreateInvite(values.email, values.meetingId);
            switch (response.status) {
                case 200:
                    form.setErrors({ email: 'Zaproszenie zostało wysłane' });
                    setRefreshData(true);
                    break;
                case 204:
                    form.setErrors({ email: 'Zaproszenie już zostało wysłane' });
                    setRefreshData(true);
                    break;
                default:
                    form.setErrors({ email: 'Wystąpił nieznany błąd' });
                    break;
            }
        } catch (error: any) {
            console.error('Error in handleInviteForm:', error);
            switch (error.code) {
                case 200:
                    form.setErrors({ email: 'Zaproszenie zostało wysłane' });
                    break;
                case 204:
                    form.setErrors({ email: 'Zaproszenie już zostało wysłane' });
                    break;
                case 401:
                    form.setErrors({ email: 'Nie masz uprawnień do zapraszania' });
                    break;
                case 401:
                    form.setErrors({ email: 'Zaproszenie już zostało wysłane' });
                    break;
                case 404:
                    form.setErrors({ email: 'Użytkownik nie istnieje' });
                    break;
                case 500:
                    form.setErrors({ email: 'Wystąpił wewnętrzny błąd serwera. Spróbuj ponownie później' });
                    break;
                default:
                    form.setErrors({ email: 'Wystąpił nieznany błąd' });
                    break;
            }
        }
    };

    const handleDeleteInvitation = async (userId: string, userName: string) => {
        try {
            const confirmed = await handleOpenModal(userName);  // Await the result of the modal
            if (!confirmed) {
                return;  // Exit the function if user cancelled
            }
            // Continue with the deletion if confirmed
            await DeleteInvitation(userId);
            setRefreshData(true);
            setPendingUsers((prev) => prev.filter((user) => user.personId !== userId));

        } catch (error) {
            console.error('Error deleting invitation:', error);
        }
    };

    const handleDeleteUserMeeting = async (userId: string, meetingId : string, userName : string) => {
        try {
            const confirmed = await handleOpenModal(userName);  // Await the result of the modal
            if (!confirmed) {
                return;  // Exit the function if user cancelled
            }
            await DeleteUserMeeting(meetingId, userId);
            setRefreshData(true);
            setInvitedUsers((prev) => prev.filter((user) => user.personId !== userId));
        } catch (error) {
            console.error('Error deleting invitation:', error);
        }
    };

    return (
        <>
            <Text size="xl" c="dimmed" fw={500}>Zaproś do spotkania</Text>
            <Text c="dimmed">Aby móc zaprosić kogoś do spotkania, osoba ta musi mieć konto w serwisie.</Text>
            <form onSubmit={form.onSubmit(handleInviteForm)}>
                <TextInput
                    placeholder="Adres email"
                    {...form.getInputProps('email')}
                />
                <Button type="submit" mt="md">Wyślij zaproszenie</Button>
            </form>

            {invitedUsers.length > 0 && (
                <>
                    <Text size="xl" c="dimmed" fw={500}>Zaakceptowane zaproszenia</Text>
                    <Flex wrap="wrap" gap="xs">
                        <AvatarGroup>
                            {invitedUsers.slice(0, 5).map(invite => (
                                <>
                                    <Tooltip key={invite.personId} label={invite.personId === data.ownerId ? invite.name+" (Właściciel)" : invite.name}
                                             transitionProps={{ transition: 'fade', duration: 300 }}>
                                        <Avatar src={`/avatars/${invite.avatar}.png`} size={50} alt={invite.name} />
                                    </Tooltip>
                                    { data.isRequestingUserAnOwner && invite.personId !== data.ownerId ? (
                                        <ActionIcon
                                            variant="filled"
                                            color="red"
                                            aria-label="Usuń zaproszenie"
                                            onClick={() => handleDeleteUserMeeting(invite.personId, data.id, invite.name)}
                                        >
                                            <IconUserMinus style={{ width: '70%', height: '70%' }} stroke={1.5} />
                                        </ActionIcon>
                                    ) : (
                                        ''
                                    )}
                                </>
                            ))}
                            {invitedUsers.length > 5 && (
                                <Tooltip
                                    withArrow
                                    label={
                                        <>
                                            {invitedUsers.slice(5).map(invite => (
                                                <div key={invite.personId}>{invite.name}</div>
                                            ))}
                                        </>
                                    }
                                >
                                    <Avatar radius="xl">+{invitedUsers.length - 5}</Avatar>
                                </Tooltip>
                            )}
                        </AvatarGroup>
                    </Flex>
                </>
            )}

            {pendingUsers.length > 0 && (
                <>
                    <Text size="xl" c="dimmed" fw={500}>Oczekujące zaproszenia</Text>
                    <Flex wrap="wrap" gap="xs">
                        <AvatarGroup>
                            {pendingUsers.slice(0, 20).map(invite => (
                                <React.Fragment key={invite.personId}>
                                    <Tooltip label={invite.name} transitionProps={{ transition: 'fade', duration: 300 }}>
                                        <Avatar src={`/avatars/${invite.avatar}.png`} size={50} alt={invite.name} />
                                    </Tooltip>
                                    {typeof invite.invitationId === 'string' && data.isRequestingUserAnOwner ? (
                                        <ActionIcon
                                            variant="filled"
                                            color="red"
                                            aria-label="Usuń zaproszenie"
                                            onClick={() => handleDeleteInvitation(invite.invitationId ?? '', invite.name)}
                                        >
                                            <IconUserMinus style={{ width: '70%', height: '70%' }} stroke={1.5} />
                                        </ActionIcon>
                                    ) : null}
                                </React.Fragment>
                            ))}
                            {pendingUsers.length > 20 && (
                                <Tooltip
                                    withArrow
                                    label={
                                        <>
                                            {pendingUsers.slice(20).map(invite => (
                                                <div key={invite.personId}>{invite.name}</div>
                                            ))}
                                        </>
                                    }
                                >
                                    <Avatar radius="xl">+{pendingUsers.length - 20}</Avatar>
                                </Tooltip>
                            )}
                        </AvatarGroup>
                    </Flex>
                </>
            )}
        </>
    );
};

export default InviteWidget;
