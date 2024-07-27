import React, {useState} from "react";
import {Button, Container, Group, rem, Title} from "@mantine/core";
import Link from "next/link";
import {cabinet} from "@/app/fonts";
import classes from './HomeNav.module.css';

const links = [
    {link: '#home', label: 'Dołącz'},
    {link: '#functions', label: 'Funkcje'},
    {link: '#invite', label: 'Zaproś znajomych'},
];

const HomeNav: React.FC = () => {
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
                <Link href="/public">
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
