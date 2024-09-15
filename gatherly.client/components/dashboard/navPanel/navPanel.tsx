"use client"
import React, {useEffect, useState} from "react";
import {Avatar, Group, Indicator, rem, Text, UnstyledButton} from '@mantine/core';
import {
    IconHome2,
    IconListCheck,
    IconLogout,
    IconMail,
    IconSettings,
    IconUsersGroup,
} from '@tabler/icons-react';
import classes from './NavPanel.module.css';
import Link from "next/link";
import LogoutUser from "@/lib/auth/logoutUser";
import JwtTokenValid from "@/lib/auth/GetUserInfo";
import {UserInfo} from "@/lib/interfaces/types";
import axiosInstance from "@/lib/utils/AxiosInstance";

const data = [
    {link: '/home/', label: 'Home', icon: IconHome2},
    {link: '/meetings/', label: 'Spotkania', icon: IconUsersGroup},
    {link: '/invitations', label: 'Zaproszenia', icon: IconMail},
    {link: '/reminders/', label: 'Przypomnienia', icon: IconListCheck},
];

const NavPanel = () => {
    const [active, setActive] = useState('');
    const [userInfo, setUserInfo] = useState<UserInfo | null>(null);
    const [invitation, setInvitation] = useState<boolean>();

    useEffect(() => {
        const fetchInvitationStatus = async () => {
            try {
                const response = await axiosInstance.get(`Invitations/invitations`);
                setInvitation(response.data);
            } catch (error) {
                console.error('Error fetching meeting status:', error);
            }
        };
        fetchInvitationStatus();
    }, []);

    useEffect(() => {
        (function() {
            const currentPath = window.location.pathname;
            const lastSegment = currentPath.split('/').filter(Boolean).pop();
            const matchedItem = data.find(item => {
                const itemPath = item.link.replace(/\//g, '');
                return itemPath === lastSegment;
            });
            if (matchedItem) {
                setActive(matchedItem.label);
            }
        })();
    }, []);

    useEffect(() => {
        const fetchUserInfo = async () => {
            const x = await JwtTokenValid();
            if (x) {
                setUserInfo({
                    name: x.name,
                    email: x.email,
                    avatarName: x.avatarName,
                });
            }
        };
        fetchUserInfo();
    }, []);

    if (!userInfo) {
        return null;
    }

    const links = data.map((item) => (
        <Link
            className={classes.link}
            data-active={item.label === active || undefined}
            href={item.link}
            key={item.label}
            onClick={(event) => {
                setActive(item.label);
            }}
        >
            {item.link == "/invitations" && invitation == true ? (<Indicator offset={-20} processing/>) : ('')}
            <item.icon className={classes.linkIcon} stroke={1.5}/>
            <span>{item.label}</span>
        </Link>
    ));

    return (
        <div className={classes.navbar}>
            <div className={classes.navbarMain}>
                {links}
            </div>

            <div className={classes.footer}>
                <UnstyledButton>
                <Group p={rem(10)}>
                    <Avatar component={"a"} href="/settings" radius="xl" size="lg" src={"/avatars/"+ userInfo.avatarName +".png"} ></Avatar>
                    <div style={{flex: 1}}>
                        <Text size="lg" fw={500}>
                            {userInfo.name}
                        </Text>
                    </div>
                </Group>
            </UnstyledButton>
                <a href="/settings" className={classes.link}>
                    <IconSettings className={classes.linkIcon} stroke={1.5}/>
                    <span>Ustawienia</span>
                </a>

                <a className={classes.link} onClick={(event) => LogoutUser()}>
                    <IconLogout className={classes.linkIcon} stroke={1.5}/>
                    <span>Wyloguj się</span>
                </a>
            </div>
        </div>

    )
}

export default NavPanel;
