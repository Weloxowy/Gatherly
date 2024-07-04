// Import styles of packages that you've installed.
// All packages except `@mantine/hooks` require styles imports
import '@mantine/core/styles.css';
import '@mantine/spotlight/styles.css';
import "./globals.css";
import {ColorSchemeScript, createTheme, MantineProvider, rem} from '@mantine/core';
import {cabinet, satoshi} from "@/app/fonts";
import React from "react";

export const metadata = {
    title: 'Gatherly',
    description: 'Planowanie spotkania nigdy nie było prostsze!'
};

const theme = createTheme({
    focusRing: "auto",
    fontSmoothing: true,
    white: "#ffffff",
    black: "#070707",
    primaryColor: "violet",
    primaryShade: 7,
    defaultRadius: "sm",
    respectReducedMotion: true,
    shadows: {
        md: '1px 1px 3px rgba(0, 0, 0, .25)',
        xl: '5px 5px 3px rgba(0, 0, 0, .25)',
    },
    headings: {
        fontFamily: cabinet.style.fontFamily,
        fontWeight: "700",
        textWrap: "balance"
    },
    fontFamily: satoshi.style.fontFamily,
});

export default function RootLayout({children}: Readonly<{ children: React.ReactNode; }>) {
    return (
        <html lang="en" style={{scrollBehavior: 'smooth'}}>
        <head >
            <ColorSchemeScript/><title></title>
        </head>
        <body>
        <MantineProvider forceColorScheme={'light'} theme={theme}>{children}</MantineProvider>
        </body>
        </html>

    );
}
