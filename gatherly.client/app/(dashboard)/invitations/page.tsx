"use client"
import classes from "./Invitations.module.css";
import { Button, Group, Paper, Title, LoadingOverlay } from "@mantine/core";
import React, { useEffect, useState } from "react";
import { InvitationMeeting } from "@/lib/interfaces/types";
import getInvitationsByUser from "@/lib/widgets/Invitations/GetInvitationsByUser";
import InvitationItem from "@/components/dashboard/InvitationItem/InvitationItem";
import {addNotification} from "@/lib/utils/notificationsManager";

export default function Invitations() {
    const [invitations, setInvitations] = useState<InvitationMeeting[]>([]);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        const fetchInvitations = async () => {
            try {
                setLoading(true);
                const response = await getInvitationsByUser();
                setInvitations(response);
            } catch (error) {
                addNotification({
                    title: 'Wystąpił błąd',
                    message: 'Zaproszenia nie zostały załadowane pomyślnie.',
                    color: 'red',
                });
                //console.error("Failed to get invitations for logged user", error);
            } finally {
                setLoading(false);
            }
        };

        fetchInvitations();
    }, []);

    return (
        <div className={classes.main}>
            <div className={classes.name}>
                <Title order={2}>Zaproszenia</Title>
                Tutaj znajdziesz wszystkie swoje zaproszenia na spotkania
            </div>
            <div style={{
                position: 'relative',
                width: '70vw',
                height: 'calc(90vh - 5rem)',
                zIndex: 10,
                display: 'flex',
                flexDirection: 'column',
                gap: '1rem'
            }}>
                <LoadingOverlay
                    visible={loading}
                    zIndex={1000}
                    overlayProps={{ radius: 'sm', blur: 2 }}
                    loaderProps={{ color: 'violet', type: 'bars' }}
                />
                { !loading && (invitations.length > 0 ? (
                    invitations.map((invitationMeeting) => (
                        <InvitationItem
                            key={invitationMeeting.InvitationId}
                            data={invitationMeeting}
                        />
                    ))
                ) : (
                    <div style={{
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        height: '100%',
                        minHeight: '100%',
                    }}>
                        Nie masz żadnych zaproszeń.
                    </div>
                ))}
            </div>
        </div>
    );
}
