module.exports = {
    presets: [
        ['@babel/preset-env', {
            targets: {
                node: 'current',
            },
        }],
        '@babel/preset-react',
        '@babel/preset-typescript',
        'next/babel', // Dodajemy `next/babel`, aby upewnić się, że Next.js działa poprawnie
    ],
    plugins: [
        ['@babel/plugin-transform-runtime', {
            runtime: 'automatic',
            regenerator: true,
        }],
    ],
};
