import React from "react";
import {Anchor, Button, Group, PasswordInput, TextInput} from "@mantine/core";
import {useForm} from "@mantine/form";

import registerUser from "@/lib/auth/RegisterUser";
import {AuthProps} from "@/lib/interfaces/types";

const Register: React.FC<AuthProps> = ({setAuthMethod, options}) => {
    const form = useForm({
        initialValues: {
            name: '', email: '', password: '',
        }, validate: {
            //email: (value) => (/^\S+@\S+$/.test(value) ? null : 'Nieprawidłowy adres email'),
            password: (value) => (value.length > 7 ? null : 'Za krótkie hasło')
        },
    });

    const handleSubmitForm = async (values: { name: string, email: string, password: string }) => {
        try {
            const res = await registerUser(values.name, values.email, values.password);
            window.location.href = "/home";
        } catch (error: any) {
            console.error('Error in handleSubmitForm:', error);
            switch (error.status) {
                case 400:
                    form.setErrors({email: 'Istnieje już użytkownik o takim adresie email'});
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
                <TextInput size="md" label="Imię" {...form.getInputProps('name')} required/>
                <TextInput size="md" label="Email" {...form.getInputProps('email')} placeholder="mail@gatherly.com"
                           required/>
                <PasswordInput size="md" label="Hasło" {...form.getInputProps('password')} required/>
                <Group justify="space-between" mt="lg">
                    <Anchor component="div" size="md" onClick={() => setAuthMethod(options.loginTraditional)}>
                        Logowanie
                    </Anchor>
                    <Anchor component="div" size="md" onClick={() => setAuthMethod(options.recover)}>
                        Odzyskaj konto
                    </Anchor>
                </Group>
                <Button fullWidth mt="lg" type="submit">
                    Zarejestruj się
                </Button>
            </form>
        </>)
}

export default Register;
