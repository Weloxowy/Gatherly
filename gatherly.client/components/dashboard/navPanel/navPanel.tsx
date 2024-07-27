"use client"
import React, {useState} from "react";
import {Avatar, Group, rem, Text, Title, UnstyledButton} from '@mantine/core';
import {
    IconHome2,
    IconListCheck,
    IconLogout,
    IconMail,
    IconSettings,
    IconUsersGroup,
} from '@tabler/icons-react';
import classes from './NavPanel.module.css';
import {cabinet} from "@/app/fonts";
import Link from "next/link";

const data = [
    {link: '/home/', label: 'Home', icon: IconHome2},
    {link: '/meetings/', label: 'Spotkania', icon: IconUsersGroup},
    {link: '', label: 'Zaproszenia', icon: IconMail},
    {link: '/reminders/', label: 'Przypomnienia', icon: IconListCheck},
];

const NavPanel = () => {
    const [active, setActive] = useState('Home');

    const links = data.map((item) => (
        <Link
            className={classes.link}
            data-active={item.label === active || undefined}
            href={item.link}
            key={item.label}
            onClick={(event) => {
                //event.preventDefault();
                setActive(item.label);
            }}
        >
            <item.icon className={classes.linkIcon} stroke={1.5}/>
            <span>{item.label}</span>
        </Link>
    ));

    return (
        <nav className={classes.navbar}>
            <div className={classes.navbarMain}>
                <Group className={classes.header} justify="space-between">
                    <Title className={cabinet.className} size={rem(32)} pl={rem(16)}>
                        Gatherly
                    </Title>

                </Group>
                {links}
            </div>

            <div className={classes.footer}>
                <UnstyledButton>
                <Group p={rem(10)}>
                    <Avatar radius="xl" size="lg" src="/avatars/avatar13.png" ></Avatar>
                    <div style={{flex: 1}}>
                        <Text size="lg" fw={500}>
                            Anna Wiech
                        </Text>
                        <Text c="dimmed" size="sm">
                            awiech@wp.pl
                        </Text>
                    </div>
                </Group>
            </UnstyledButton>
                <a href="#" className={classes.link} onClick={(event) => event.preventDefault()}>
                    <IconSettings className={classes.linkIcon} stroke={1.5}/>
                    <span>Ustawienia</span>
                </a>

                <a href="#" className={classes.link} onClick={(event) => event.preventDefault()}>
                    <IconLogout className={classes.linkIcon} stroke={1.5}/>
                    <span>Wyloguj się</span>
                </a>
            </div>
        </nav>

    )
}

export default NavPanel;
