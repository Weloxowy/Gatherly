import { Inter} from 'next/font/google'
import localFont from 'next/font/local'
export const inter = Inter({
    subsets: ['latin'],
    display: 'swap',
    variable: '--font-inter',
})

export const cabinet = localFont({
    src: '../public/fonts/CabinetGrotesk-Variable.woff2',
    display: 'swap',
    variable: '--font-cabinet',
})

export const satoshi = localFont({
    src: '../public/fonts/Satoshi-Variable.woff2',
    display: 'swap',
    variable: '--font-satoshi',
})
