import React from 'react';
import LoginByCode from './LoginByCode';
import sendSsoCode from '../../lib/auth/sendSsoCode';
import sendReturnedCode from '../../lib/auth/sendReturnedCode';
import { jest } from '@jest/globals';
import '@testing-library/jest-dom';
import { render, screen, fireEvent, waitFor } from '../../test/render';

// Function types
type SendSsoCode = (email: string) => Promise<any>;
type SendReturnedCode = (email: string, code: string) => Promise<any>;

// Mock the auth functions
const mockedSendSsoCode = sendSsoCode as jest.MockedFunction<SendSsoCode>;
const mockedSendReturnedCode = sendReturnedCode as jest.MockedFunction<SendReturnedCode>;

jest.mock('../../lib/auth/sendSsoCode', () => ({
    __esModule: true,
    default: mockedSendSsoCode,
}));
jest.mock('../../lib/auth/sendReturnedCode', () => ({
    __esModule: true,
    default: mockedSendReturnedCode,
}));

const setAuthMethodMock = jest.fn();
const optionsMock = {
    loginTraditional: 'loginTraditional',
    loginByCode: 'loginByCode',
    register: 'register',
    recover: 'recover'
};

describe('LoginByCode component', () => {
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('renders the email form (step 1)', () => {
        render(<LoginByCode setAuthMethod={setAuthMethodMock} options={optionsMock} />);
        expect(screen.getByLabelText(/Email/i)).toBeInTheDocument();
        expect(screen.getByText(/Logowanie tradycyjne/i)).toBeInTheDocument();
        expect(screen.getByText(/Odzyskaj konto/i)).toBeInTheDocument();
    });

    test('submits the email form successfully', async () => {
        mockedSendSsoCode.mockResolvedValue({});

        render(<LoginByCode setAuthMethod={setAuthMethodMock} options={optionsMock} />);

        const emailInput = screen.getByLabelText(/Email/i) as HTMLInputElement;
        fireEvent.change(emailInput, { target: { value: 'test@mail.com' } });
        fireEvent.click(screen.getByText(/Zaloguj/i));

        await waitFor(() => expect(mockedSendSsoCode).toHaveBeenCalledWith('test@mail.com'));
        expect(screen.getByText(/Zatwierdź kod/i)).toBeInTheDocument();
    });

    test.each([
        [400, 'Podano nieprawidłowy adres'],
        [404, 'Nie znaleziono takiego adresu'],
        [500, 'Wystąpił wewnętrzny błąd serwera. Spróbuj ponownie później'],
        [999, 'Wystąpił nieznany błąd']
    ])('handles email submission error %i', async (errorCode, expectedMessage) => {
        mockedSendSsoCode.mockRejectedValue({ code: errorCode });

        render(<LoginByCode setAuthMethod={setAuthMethodMock} options={optionsMock} />);

        const emailInput = screen.getByLabelText(/Email/i) as HTMLInputElement;
        fireEvent.change(emailInput, { target: { value: 'test@mail.com' } });
        fireEvent.click(screen.getByText(/Zaloguj/i));

        await waitFor(() => expect(mockedSendSsoCode).toHaveBeenCalledWith('test@mail.com'));
        expect(screen.getByText(expectedMessage)).toBeInTheDocument();
    });

    test('submits the code form successfully', async () => {
        mockedSendReturnedCode.mockResolvedValue({});

        render(<LoginByCode setAuthMethod={setAuthMethodMock} options={optionsMock} />);

        const emailInput = screen.getByLabelText(/Email/i) as HTMLInputElement;
        fireEvent.change(emailInput, { target: { value: 'test@mail.com' } });
        fireEvent.click(screen.getByText(/Zaloguj/i));
        await waitFor(() => expect(screen.getByText(/Zatwierdź kod/i)).toBeInTheDocument());

        const codeInput = screen.getByRole('textbox') as HTMLInputElement;
        fireEvent.change(codeInput, { target: { value: '123456' } });
        fireEvent.click(screen.getByText(/Zatwierdź kod/i));

        await waitFor(() => expect(mockedSendReturnedCode).toHaveBeenCalledWith('test@mail.com', '123456'));
    });

    test.each([
        [400, 'Podano nieprawidłowy adres'],
        [404, 'Błędny kod. Spróbuj ponownie'],
        [500, 'Wystąpił wewnętrzny błąd serwera. Spróbuj ponownie później'],
        [999, 'Wystąpił nieznany błąd']
    ])('handles code submission error %i', async (errorCode, expectedMessage) => {
        mockedSendReturnedCode.mockRejectedValue({ code: errorCode });

        render(<LoginByCode setAuthMethod={setAuthMethodMock} options={optionsMock} />);

        const emailInput = screen.getByLabelText(/Email/i) as HTMLInputElement;
        fireEvent.change(emailInput, { target: { value: 'test@mail.com' } });
        fireEvent.click(screen.getByText(/Zaloguj/i));
        await waitFor(() => expect(screen.getByText(/Zatwierdź kod/i)).toBeInTheDocument());

        const codeInput = screen.getByRole('textbox') as HTMLInputElement;
        fireEvent.change(codeInput, { target: { value: '123456' } });
        fireEvent.click(screen.getByText(/Zatwierdź kod/i));

        await waitFor(() => expect(mockedSendReturnedCode).toHaveBeenCalledWith('test@mail.com', '123456'));
        expect(screen.getByText(expectedMessage)).toBeInTheDocument();
    });

    test('clicks on the "Logowanie tradycyjne" link', () => {
        render(<LoginByCode setAuthMethod={setAuthMethodMock} options={optionsMock} />);
        fireEvent.click(screen.getByText(/Logowanie tradycyjne/i));
        expect(setAuthMethodMock).toHaveBeenCalledWith(optionsMock.loginTraditional);
    });

    test('clicks on the "Odzyskaj konto" link', () => {
        render(<LoginByCode setAuthMethod={setAuthMethodMock} options={optionsMock} />);
        fireEvent.click(screen.getByText(/Odzyskaj konto/i));
        expect(setAuthMethodMock).toHaveBeenCalledWith(optionsMock.recover);
    });

    test('changes input values in both forms', async () => {
        render(<LoginByCode setAuthMethod={setAuthMethodMock} options={optionsMock} />);

        const emailInput = screen.getByLabelText(/Email/i) as HTMLInputElement;
        fireEvent.change(emailInput, { target: { value: 'test@mail.com' } });
        expect(emailInput.value).toBe('test@mail.com');

        fireEvent.click(screen.getByText(/Zaloguj/i));
        await waitFor(() => expect(screen.getByText(/Zatwierdź kod/i)).toBeInTheDocument());

        const codeInput = screen.getByRole('textbox') as HTMLInputElement;
        fireEvent.change(codeInput, { target: { value: '123456' } });
        expect(codeInput.value).toBe('123456');
    });
});
