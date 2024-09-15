"use client"
import React, {useEffect, useState} from "react";
import classes from "@/app/(dashboard)/home/Home.module.css";
import {Avatar, Button, Group, Paper, Space, Text, Title} from "@mantine/core";
import {closeAllModals, modals, openModal} from "@mantine/modals";
import NewMeeting from "@/components/dashboard/NewMeeting/NewMeeting";
import ChangeBasicUserData from "@/components/widgets/Settings/ChangeBasicUserData";
import JwtTokenValid from "@/lib/auth/GetUserInfo";
import axiosInstance from "@/lib/utils/AxiosInstance";
import LogoutUser from "@/lib/auth/logoutUser";

export default function Settings() {
    const [mode, setMode] = useState('');
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const [avatar, setAvatar] = useState('');

    useEffect(() => {
        const fetchUserInfo = async () => {
            const x = await JwtTokenValid();
            if (x) {
                setName(x.name);
                setEmail(x.email);
                setAvatar(x.avatarName);
            }
        };
        fetchUserInfo();
    }, []);

    const handleChangeDataModal = () => {
        openModal({
            title: <Title order={2}>Zmień dane użytkownika</Title>,
            size: '70%',
            radius: 10,
            children: (<ChangeBasicUserData/>),
        });
    };

    const handleDeleteAccountModal = () => modals.openConfirmModal({
        title: <Title order={2}>Usunięcie profilu</Title>,
        size: '70%',
        radius: 10,
        children: (<Text size="sm">
            Aby usunąć profil kliknij przycisk potwierdź. Operacja jest nieodwracalna.
        </Text>),
        labels: {confirm: 'Potwierdź', cancel: 'Anuluj'},
        onCancel: () => closeAllModals(),
        onConfirm: () => {
            axiosInstance.delete('/User/profile').then(r => r.status);
            closeAllModals();
            LogoutUser();
        },
    });


    return (<div className={classes.main}>
        <div className={classes.name}>
            <Title order={2}>Ustawienia</Title>
            Zmień swój awatar, podejrzyj szczegóły konta
        </div>
        <Paper radius="lg" shadow="lg" p="lg" className={classes.componentMd} m="lg">
            <Title order={3}>Twoje dane</Title>
            <Space h="lg"/>
            <Group style={{alignContent: "stretch",alignItems:"center"}}>
                <Text size="lg">
                    {name} <br/>
                    {email}<br/>
                </Text>
                <Avatar size={120} src={"/avatars/" + avatar + ".png"}/>
            </Group>
            <Space h="lg"/>
            <Group>
                <Button onClick={handleChangeDataModal}>Zmień podstawowe dane konta</Button>
                <Button onClick={handleDeleteAccountModal}>Usuń konto</Button>
            </Group>
        </Paper>
    </div>);
}
