import React from "react";
import {Anchor, Button, Checkbox, Group, PasswordInput, TextInput, Title} from "@mantine/core";
import {useForm} from "@mantine/form";
import {Struct} from "next/dist/compiled/superstruct";
import loginValid from "@/lib/auth/LoginValid";

const LoginTraditional : React.FC = ({setAuthMethod, options} : Struct<object, string>) => {
    const form = useForm({
        initialValues: {
            email: '',
            password: '',
        },
        validate: {
            email: (value) => (/^\S+@\S+$/.test(value) ? null : 'Nieprawidłowy adres email'),
        },
    });

    const handleSubmitForm = async (values: { email: string, password: string }) => {
        try {
            const res = await loginValid(values.email, values.password);
            window.location.href = "/testLogin";
        } catch (error: any) {
            console.error('Error in handleSubmitForm:', error);
            switch (error.code) {
                case 400:
                case 404:
                    form.setErrors({ password: 'Podano nieprawidłowy dane uwierzytelniające' });
                    break;
                case 500:
                    form.setErrors({ password: 'Wystąpił wewnętrzny błąd serwera. Spróbuj ponownie później' });
                    break;
                default:
                    form.setErrors({ password: 'Wystąpił nieznany błąd' });
                    break;
            }
        }
    };

    return (
            <>
                <form onSubmit={form.onSubmit(handleSubmitForm)}>
                <TextInput size="md" label="Email" placeholder="mail@gatherly.com"
                           {...form.getInputProps('email')} required/>
                <PasswordInput size="md" label="Password"
                               {...form.getInputProps('password')} required/>
                <Group justify="space-between" mt="lg">
                    <Anchor component="button" size="md" onClick={() => setAuthMethod(options.loginByCode)}>
                        Logowanie jednorazowe
                    </Anchor>
                    <Anchor component="button" size="md" onClick={() => setAuthMethod(options.register)}>
                        Zarejestruj się
                    </Anchor>
                </Group>
                    <Button fullWidth mt="lg" type="submit">
                        Zaloguj
                    </Button>
                </form>
            </>
        );
}

export default LoginTraditional;
