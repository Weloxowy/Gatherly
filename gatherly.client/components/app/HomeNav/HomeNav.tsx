
import React from "react";
import {
    Button, rem,
    Title
} from "@mantine/core";
import Link from "next/link";
import {cabinet} from "@/app/fonts";
import { useState } from 'react';
import { Container, Group } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import classes from './HomeNav.module.css';

const links = [
    { link: '#home', label: 'Dołącz' },
    { link: '#functions', label: 'Funkcje' },
    { link: '#invite', label: 'Zaproś znajomych' },
];

const HomeNav: React.FC = () => {
    const [opened, { toggle }] = useDisclosure(false);
    const [active, setActive] = useState(links[0].link);

    const items = links.map((link) => (
        <Link
            key={link.label}
            href={link.link}
            className={classes.link}
            data-active={active === link.link || undefined}
        >
            {link.label}
        </Link>
    ));

    return (
        <header className={classes.header}>
            <Container size="md" className={classes.inner}>
                <Link href="/">
                    <Title className={cabinet.className} size={rem(40)}>
                        Gatherly
                    </Title>
                </Link>
                <Group gap={20} visibleFrom="sm">
                    {items}
                </Group>
                <Button component={Link} href={"/auth"} variant="outline" size={"sm"}>
                    Dołącz
                </Button>
            </Container>
        </header>
    );
}
export default HomeNav;
