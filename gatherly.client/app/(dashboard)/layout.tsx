"use client"
import {AppShell, Burger, Flex, Group, rem, Title} from "@mantine/core";
import React from "react";
import {useDisclosure} from "@mantine/hooks";
import classes from "@/components/dashboard/navPanel/NavPanel.module.css";
import {cabinet} from "@/app/fonts";
import dynamic from 'next/dynamic';

const NavPanel = dynamic(() => import('@/components/dashboard/navPanel/navPanel'), {
    ssr: false
});
export default function DashboardLayout({
                                            children,
                                        }: {
    children: React.ReactNode
}) {

    const [opened, {toggle}] = useDisclosure();
    return (
        <AppShell
            header={{ height: { base: 60, sm: 70, md: 70, lg: 80 }}}
            navbar={{
                width: { base: 250, sm: 250, md: 250, lg: 300 },
                breakpoint: 'sm',
                collapsed: { mobile: !opened },
            }}
            padding="md"
        >
            <AppShell.Header
                w={{ base: '100vw',sm: 250, md: 250, lg: 300 }}
                style={{
                    borderInlineEnd: 'calc(0.0625rem * var(--mantine-scale)) solid var(--app-shell-border-color)',
                    '@media (maxWidth: 1300px)': {
                        borderInlineEnd: 'none'
                    }
                }}
            >
                    <Group className={classes.header} justify="space-between" h="100%" px="md" align="center">
                        <Title className={cabinet.className} size={rem(40)} pl={rem(16)}>
                            Gatherly
                        </Title>
                        <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" />
                    </Group>
            </AppShell.Header>
            <AppShell.Navbar h={"100%"}>
                <NavPanel/>
            </AppShell.Navbar>
            <AppShell.Main pt={rem(16)}>
                    {children}
            </AppShell.Main>
        </AppShell>
    )
}
