/** @type {import('next').NextConfig} */
const nextConfig = {
    reactStrictMode: false,
    optimizeFonts: true,
    env:{
        MAP_KEY : process.env.MAP_KEY,
        DB : process.env.DB
    },
    /*
    i18n: {
        // These are all the locales you want to support in
        // your application
        locales: ['en', 'pl-PL'],
        // This is the default locale you want to be used when visiting
        // a non-locale prefixed path e.g. `/hello`
        defaultLocale: 'pl-PL',
    },
    */
    experimental: {
        optimizePackageImports: ['@mantine/core', '@mantine/hooks'],
        forceSwcTransforms: true,
        optimizeServerReact: true
    },
    eslint: {
        ignoreDuringBuilds: true, // Wyłącza ESLint podczas budowania
    },
/*
    output: 'export', // Dodaje eksport jako statyczną stronę
    distDir: 'out',   // Określenie katalogu dla zbudowanych plików
*/
};



export default nextConfig;
