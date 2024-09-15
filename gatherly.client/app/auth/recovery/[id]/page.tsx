"use client"
import {Button, Container, Paper, PasswordInput, Text, TextInput, Title} from '@mantine/core';
import React, {useEffect, useState} from "react";
import {useForm} from "@mantine/form";
import sendOpeningOfRecovery from "@/lib/auth/sendOpeningOfRecovery";
import sendRecoveryChanges from "@/lib/auth/sendRecoveryChanges";

export default function Recovery() {
    const [responseMessage, setResponseMessage] = useState('');

    const form = useForm({
        initialValues: {
            email: '', password: '', confirmPassword: ''
        }, validate: {
            email: (value) => (/^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/.test(value) ? null : 'Nieprawidłowy adres email'),
            password: (value, values) => value === values.confirmPassword ? null : 'Hasła nie są identyczne',
            confirmPassword: (value, values) => value === values.password ? null : 'Hasła nie są identyczne'
        },
    });

    useEffect(() => {
        (async () => {
            const currentUrl = window.location.href;
            const id = currentUrl.substring(currentUrl.lastIndexOf('/') + 1);
            await sendOpeningOfRecovery(id);
        })();
    }, []);
    const handleSubmitForm = async (values: { email: string, password: string }) => {
        try {
            const res = await sendRecoveryChanges(values.email, values.password);
            if (res.status === 200) {
                setResponseMessage('Zmiana dokonana pomyślnie. Za 5 sekund przeniesiesz się do panelu logowania.');
                setTimeout(() => {
                    window.location.href = "/auth";
                }, 5000);
            }
        } catch (error: any) {
            console.error('Error in handleSubmitForm1:', error);
            switch (error.code) {
                case 400:
                    form.setFieldError('password', 'Podano nieprawidłowy adres');
                    break;
                case 404:
                    form.setFieldError('password', 'Nie znaleziono takiego adresu');
                    break;
                case 500:
                    form.setFieldError('password', 'Wystąpił wewnętrzny błąd serwera. Spróbuj ponownie później');
                    break;
                default:
                    form.setFieldError('password', 'Wystąpił nieznany błąd');
                    break;
            }
        }
    };
    return (<main>
            <Container size="sm" my="xl" pt={100}>
                <Container ta="center" style={{alignItems: "center"}}>
                    <Title order={1} ta="center">
                        Resetowanie hasła
                    </Title>
                    <Text c="dimmed" size="md" ta="center" mt={5}>
                        Wprowadź nowe hasło i potwierdź je.
                    </Text>
                    <Paper withBorder shadow="md" p="lg" mt="xl" radius="md">
                        {responseMessage ? (<Text size="md" ta="center">
                                {responseMessage}
                            </Text>) : (<form onSubmit={form.onSubmit(handleSubmitForm)}>
                                <TextInput size="md" label="Email twojego konta"
                                           {...form.getInputProps('email')}
                                           placeholder="mail@gatherly.com" required/>
                                <PasswordInput
                                    size="md"
                                    label="Podaj nowe hasło"
                                    {...form.getInputProps('password')}
                                    required
                                />
                                <PasswordInput
                                    size="md"
                                    label="Potwierdź nowe hasło"
                                    {...form.getInputProps('confirmPassword')}
                                    required
                                />
                                <Button type="submit">Zmień hasło</Button>
                            </form>)}
                    </Paper>
                </Container>
            </Container>
        </main>);
}
