const path = require('path');

module.exports = {
    transform: {
        '^.+\\.(ts|tsx|js|jsx)$': 'babel-jest',
    },
    testEnvironment: 'jsdom',
    moduleFileExtensions: ['ts', 'tsx', 'js', 'jsx', 'json', 'node'],
    testPathIgnorePatterns: ['/node_modules/', '/.next/'],
    transformIgnorePatterns: ['/node_modules/(?!(your-module-name)/)'],
    moduleNameMapper: {
        '\\.(css|less|scss|sass)$': 'identity-obj-proxy',
        '^@/(.*)$': '<rootDir>/$1',
        'next/font/local': '<rootDir>/test/__mocks__/local.js',
    },
    setupFilesAfterEnv: [
        '<rootDir>/jest.setup.cjs',
        '<rootDir>/node_modules/@testing-library/jest-dom'
    ],
};
