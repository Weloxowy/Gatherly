import {createTheme} from "@mantine/core";
import {cabinet, satoshi} from "./fonts"

export const theme = createTheme({
    components: {
      Button : {
          defaultProps: {
              loaderProps: {type: "bars", size: 'xs'},
          }
      }
    },
    focusRing: "auto",
    fontSmoothing: true,
    white: "#feffff",
    black: "#1b1a1a",
    defaultGradient: {from: 'violet.7', to: 'teal', deg: 20},
    primaryColor: "violet",
    primaryShade: 7,
    defaultRadius: "sm",
    respectReducedMotion: true,
    shadows: {
        md: '1px 1px 3px rgba(0, 0, 0, .10)',
        lg: '1px 1px 3px rgba(0, 0, 0, .10)',
        xl: '5px 5px 3px rgba(0, 0, 0, .15)',
    },
    headings: {
        fontFamily: cabinet.style.fontFamily,
        fontWeight: "700",
        textWrap: "balance"
    },
    fontFamily: satoshi.style.fontFamily,
});
