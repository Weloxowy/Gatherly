import "./globals.css";
import '@mantine/core/styles.css';
import '@mantine/spotlight/styles.css';
import '@mantine/notifications/styles.css';

import {ColorSchemeScript, MantineProvider} from '@mantine/core';
import React from "react";
import {theme} from "@/app/theme";
import {ModalsProvider} from "@mantine/modals";
import {SpeedInsights} from "@vercel/speed-insights/next"
import {Notifications} from "@mantine/notifications";
import {NotificationsProvider} from "@/components/notifications/NotificationContext";
import { Analytics } from "@vercel/analytics/react"

export const metadata = {
    title: 'Gatherly', description: 'Planowanie spotkania nigdy nie było prostsze!'
};

export default function RootLayout({children}: Readonly<{ children: React.ReactNode; }>) {
    return (<html lang="en" style={{scrollBehavior: 'smooth'}}>
        <head>
            <ColorSchemeScript/><title></title>
            <SpeedInsights/>
            <Analytics/>
        </head>
        <body>
        <MantineProvider defaultColorScheme={"auto"} theme={theme}>
            <ModalsProvider>
                <NotificationsProvider>
                    <Notifications/>
                    {children}
                </NotificationsProvider>
            </ModalsProvider>
        </MantineProvider>
        </body>
        </html>

    );
}
