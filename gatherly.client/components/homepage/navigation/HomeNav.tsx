import React, { useState, useEffect } from "react";
import {
    Button,
    Container,
    Group,
    MantineThemeProvider,
    rem,
    Title,
    useMantineColorScheme,
    useMantineTheme
} from "@mantine/core";
import Link from "next/link";
import { cabinet } from "@/app/fonts";
import classes from './HomeNav.module.css';
import clsx from "clsx";

const links = [
    { link: '#home', label: 'Home' },
    { link: '#chat', label: 'Chat' },
    { link: '#plan', label: 'Planowanie spotkań' },
];

const HomeNav: React.FC = () => {
    const [active, setActive] = useState(links[0].link);
    const [isDarkText, setIsDarkText] = useState(false);
    const handleScroll = () => {
        const sectionThreshold = 500;
        const scrollPosition = window.scrollY;
        const colorScheme = useMantineColorScheme.toString(); // Get the current color scheme from Mantine
        const isDarkMode = colorScheme === 'dark';

        // Zmieniaj kolor tekstu, gdy przekroczono próg
        if (isDarkMode) {
            // Dla ciemnego motywu
            if (scrollPosition > sectionThreshold) {
                setIsDarkText(false); // Tekst będzie jasny przy scrollu
            } else {
                setIsDarkText(true); // Tekst ciemny na początku
            }
        } else {
            // Dla jasnego motywu
            if (scrollPosition > sectionThreshold) {
                setIsDarkText(true); // Tekst będzie ciemny przy scrollu
            } else {
                setIsDarkText(false); // Tekst jasny na początku
            }
        }

        // Ustal aktywną sekcję
        const sectionOffsets = links.map(link => {
            const section = document.querySelector(link.link);
            // @ts-ignore
            return section ? section.offsetTop : 0;
        });

        for (let i = 0; i < sectionOffsets.length; i++) {
            if (scrollPosition >= sectionOffsets[i] - 50 &&
                (i === sectionOffsets.length - 1 || scrollPosition < sectionOffsets[i + 1] - 50)) {
                setActive(links[i].link);
                break;
            }
        }
    };

    useEffect(() => {
        window.addEventListener('scroll', handleScroll);
        return () => window.removeEventListener('scroll', handleScroll);
    }, []);

    const items = links.map((link) => (
        <Link
            key={link.label}
            href={link.link}
            className={`${classes.link} ${active === link.link ? classes.activeLink : ''}`}
            data-active={active === link.link || undefined}
        >
            {link.label}
        </Link>
    ));

    return (
        <header className={clsx(classes.header, isDarkText ? classes.darkText : classes.lightText)}>
            <Container size="md" className={classes.inner}>
                <Link href="/">
                    <Title className={clsx(cabinet.className, isDarkText ? classes.darkText : classes.lightText)} size={rem(40)}>
                        Gatherly
                    </Title>
                </Link>
                <Group gap={20} visibleFrom="sm">
                    {items}
                </Group>
                <Button color={ isDarkText ? classes.darkText : classes.lightText} component={Link} href={"/auth"} variant="outline" size={"sm"}>
                    Dołącz
                </Button>
            </Container>
        </header>
    );
};

export default HomeNav;
