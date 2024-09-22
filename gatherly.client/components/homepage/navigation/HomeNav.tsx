import React, { useState, useEffect } from "react";
import {
    ActionIcon,
    Button,
    Container,
    Group,
    rem,
    Title,
    useMantineColorScheme,
} from "@mantine/core";
import Link from "next/link";
import { cabinet } from "@/app/fonts";
import classes from './HomeNav.module.css';
import clsx from "clsx";
import { IconMoonStars, IconSun } from "@tabler/icons-react";

const links = [
    { link: '#home', label: 'Home' },
    { link: '#chat', label: 'Chat' },
    { link: '#plan', label: 'Planowanie spotkań' },
];

const HomeNav: React.FC = () => {
    const [active, setActive] = useState(links[0].link);
    const { colorScheme, setColorScheme } = useMantineColorScheme();
    const [darkMode, setDarkMode] = useState(colorScheme === 'dark');
    const [isDarkText, setIsDarkText] = useState(false);

    // Funkcja zmiany motywu
    const changeMode = () => {
        setColorScheme(colorScheme === 'dark' ? 'light' : 'dark');
        setDarkMode(!darkMode);
        handleScroll();
    };

    // Funkcja obsługi zmiany szerokości okna
    const handleResize = () => {
        const windowWidth = window.innerWidth;
        if (windowWidth < 500) {
            // Motyw ciemny poniżej 500px
            setColorScheme('dark');
            setDarkMode(true);
        } else {
            // Motyw jasny powyżej 500px
            setColorScheme('light');
            setDarkMode(false);
        }
    };

    useEffect(() => {
        // Wywołaj handleResize od razu, aby ustawić motyw przy pierwszym renderze
        handleResize();

        // Nasłuchiwanie na zmiany rozmiaru okna
        window.addEventListener('resize', handleResize);

        return () => {
            window.removeEventListener('resize', handleResize);
        };
    }, []); // Pusty array [] zapewnia, że useEffect uruchomi się raz przy pierwszym renderze

    const handleScroll = () => {
        const sectionThreshold = 500;
        const scrollPosition = window.scrollY;

        // Zmieniaj kolor tekstu, gdy przekroczono próg
        if (darkMode) {
            if (scrollPosition > sectionThreshold) {
                setIsDarkText(false); // Tekst jasny przy scrollu
            } else {
                setIsDarkText(false); // Tekst ciemny na początku
            }
        } else {
            if (scrollPosition > sectionThreshold) {
                setIsDarkText(true); // Tekst ciemny przy scrollu
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
    }, [darkMode]);

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
                <ActionIcon onClick={changeMode} variant={"outline"} color={ isDarkText ? classes.darkText : classes.lightText} aria-label="Change theme">
                    {darkMode ? (
                        <IconMoonStars style={{ width: '70%', height: '70%' }} stroke={1.5} />
                    ) : (
                        <IconSun style={{ width: '70%', height: '70%' }} stroke={1.5} />
                    )}
                </ActionIcon>
            </Container>
        </header>
    );
};

export default HomeNav;
