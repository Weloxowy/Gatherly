import React from "react";
import {Anchor, Button, FocusTrap, Group, PasswordInput, TextInput} from "@mantine/core";
import {useForm} from "@mantine/form";
import loginValid from "@/lib/auth/LoginValid";
import {AuthProps} from "@/lib/interfaces/types";
import Link from "next/link";

const LoginTraditional: React.FC<AuthProps> = ({setAuthMethod, options}) => {
    const form = useForm({
        initialValues: {
            email: '', password: '',
        }, validate: {//  ^\S+@\S+\.+\S{2}
            email: (value) => (/^\S+@\S+\.+\S{2}/.test(value) ? null : 'Nieprawidłowy adres email'),
        },
    });

    const handleSubmitForm = async (values: { email: string, password: string }) => {
        try {
            await loginValid(values.email, values.password);
            window.location.href = "/autorization";
        } catch (error: any) {
            console.error('Error in handleSubmitForm:', error);
            switch (error.status) {
                case 401:
                    form.setErrors({password: 'Podano nieprawidłowy dane uwierzytelniające'});
                    break;
                case 404:
                    form.setErrors({password: 'Podany użytkownik nie istnieje'});
                    break;
                case 500:
                    form.setErrors({password: 'Wystąpił wewnętrzny błąd serwera. Spróbuj ponownie później'});
                    break;
                default:
                    form.setErrors({password: 'Wystąpił nieznany błąd'});
                    break;
            }
        }
    };

    return (<>
            <form onSubmit={form.onSubmit(handleSubmitForm)}>
                <TextInput size="md" label="Email" placeholder="mail@gatherly.com"
                           {...form.getInputProps('email')} required/>
                <PasswordInput size="md" label="Password"
                               {...form.getInputProps('password')} required/>
                <Group justify="space-between" mt="lg">
                    <Anchor component="div" size="md" onClick={() => setAuthMethod(options.loginByCode)}>
                        Logowanie jednorazowe
                    </Anchor>
                    <Anchor component="div" size="md" onClick={() => setAuthMethod(options.recover)}>
                        Odzyskaj konto
                    </Anchor>
                </Group>

                <Button fullWidth mt="lg" type="submit">
                    Zaloguj
                </Button>
            </form>
        </>);
}

export default LoginTraditional;
