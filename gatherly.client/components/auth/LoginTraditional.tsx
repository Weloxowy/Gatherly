import React from "react";
import {Anchor, Button, Group, PasswordInput, TextInput} from "@mantine/core";
import {useForm} from "@mantine/form";
import loginValid from "@/lib/auth/LoginValid";
import {AuthProps} from "@/lib/interfaces/types";

const LoginTraditional: React.FC<AuthProps> = ({setAuthMethod, options}) => {
    const form = useForm({
        initialValues: {
            email: '', password: '',
        }, validate: {
            email: (value) => (/^\S+@\S+$/.test(value) ? null : 'Nieprawidłowy adres email'),
        },
    });

    const handleSubmitForm = async (values: { email: string, password: string }) => {
        try {
            await loginValid(values.email, values.password);
            window.location.href = "/autorization";
        } catch (error: any) {
            console.error('Error in handleSubmitForm:', error);
            switch (error.code) {
                case 400:
                case 404:
                    form.setErrors({password: 'Podano nieprawidłowy dane uwierzytelniające'});
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
                    <Anchor component="button" size="md" onClick={() => setAuthMethod(options.loginByCode)}>
                        Logowanie jednorazowe
                    </Anchor>
                    <Anchor component="button" size="md" onClick={() => setAuthMethod(options.recover)}>
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
