import "./globals.css";
import '@mantine/core/styles.css';
import '@mantine/spotlight/styles.css';

import {ColorSchemeScript, MantineProvider} from '@mantine/core';
import React from "react";
import {theme} from "@/app/theme";
import {ModalsProvider} from "@mantine/modals";

export const metadata = {
    title: 'Gatherly', description: 'Planowanie spotkania nigdy nie było prostsze!'
};

export default function RootLayout({children}: Readonly<{ children: React.ReactNode; }>) {
    return (<html lang="en" style={{scrollBehavior: 'smooth'}}>
        <head>
            <ColorSchemeScript/><title></title>
        </head>
        <body>
        <MantineProvider forceColorScheme={'light'} theme={theme}>
            <ModalsProvider>
                {children}
            </ModalsProvider>
        </MantineProvider>
        </body>
        </html>

    );
}
