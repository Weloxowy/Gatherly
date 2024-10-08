﻿import {createTheme} from "@mantine/core";

export const theme = createTheme({
    focusRing: "auto",
    fontSmoothing: true,
    white: "#ffffff",
    black: "#070707",
    defaultGradient: {from: 'violet.7', to: 'teal', deg: 20},
    primaryColor: "violet",
    primaryShade: 7,
    defaultRadius: "sm",
    respectReducedMotion: true,
    shadows: {
        md: '1px 1px 3px rgba(0, 0, 0, .25)',
        xl: '5px 5px 3px rgba(0, 0, 0, .25)',
    }
});
