"use client"
import classes from "./Invitations.module.css";
import { Button, Group, Paper, Title } from "@mantine/core";
import React, { useEffect, useState } from "react";
import { InvitationMeeting } from "@/lib/interfaces/types";
import getInvitationsByUser from "@/lib/widgets/Invitations/GetInvitationsByUser";
import InvitationItem from "@/components/dashboard/InvitationItem/InvitationItem";

export default function Invitations() {
    const [invitations, setInvitations] = useState<InvitationMeeting[]>([]);

    useEffect(() => {
        const fetchInvitations = async () => {
            try {
                const response = await getInvitationsByUser();
                setInvitations(response);
            } catch (error) {
                console.error("Failed to get invitations for logged user", error);
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
                height: 'calc(100vh - 5rem)',
                zIndex: 10,
                display: 'flex',
                flexDirection: 'column',
                gap: '1rem'
            }}>
                { invitations.length > 0 ?
                    ( invitations.map((invitationMeeting) => (
                    <InvitationItem
                        key={invitationMeeting.InvitationId}
                        data={invitationMeeting}
                    />
                ) )): ('Nie masz żadnych zaproszeń.')}
            </div>
        </div>
    );
}
