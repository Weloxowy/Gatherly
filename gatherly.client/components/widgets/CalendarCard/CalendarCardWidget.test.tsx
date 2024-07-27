import React from "react";
import { render, screen } from '../../../test/render';
import '@testing-library/jest-dom';
import CalendarCardWidget from './CalendarCardWidget';

// Mock current date
const mockDate = new Date(2023, 6, 20); // July 20, 2023 (months are 0-indexed)
const RealDate = Date;

beforeAll(() => {
    global.Date = jest.fn(() => mockDate) as unknown as DateConstructor;
    global.Date.now = RealDate.now;
    global.Date.UTC = RealDate.UTC;
    global.Date.parse = RealDate.parse;
    global.Date.toString = RealDate.toString;
});

afterAll(() => {
    global.Date = RealDate;
});

test('renders the correct day, date, and month', () => {
    render(<CalendarCardWidget />);

    expect(screen.getByText('Czwartek')).toBeInTheDocument();
    expect(screen.getByText('20')).toBeInTheDocument();
    expect(screen.getByText('Lipiec')).toBeInTheDocument();
    expect(screen.getByText('Masz dziś 0 spotkań.')).toBeInTheDocument();
});
