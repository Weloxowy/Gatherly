/** @type {import('next').NextConfig} */
const nextConfig = {
    i18n: {
        // These are all the locales you want to support in
        // your application
        locales: ['en', 'pl-PL'],
        // This is the default locale you want to be used when visiting
        // a non-locale prefixed path e.g. `/hello`
        defaultLocale: 'pl-PL',
    },
    experimental: {
        optimizePackageImports: ['@mantine/core', '@mantine/hooks'],
    }
};



export default nextConfig;
