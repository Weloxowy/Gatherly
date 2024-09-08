import React, {useState} from "react";
import {Anchor, Button, Group, Text, TextInput} from "@mantine/core";
import {AuthProps} from "@/lib/interfaces/types";
import sendRecoveryRequest from "@/lib/auth/sendRecoveryRequest";
import {useForm} from "@mantine/form";

const Recover: React.FC<AuthProps> = ({setAuthMethod, options}) => {
    const [responseMessage, setResponseMessage] = useState(''); // Dodany stan do przechowywania odpowiedzi
    const form = useForm({
        initialValues: {
            email: '',
        }, validate: {
            email: (value) => (/^\S+@\S+$/.test(value) ? null : 'Nieprawidłowy adres email'),
        },
    });

    const handleSubmitForm = async (values: { email: string }) => {
        try {
            const res = await sendRecoveryRequest(values.email);

            if (res.status === 200) {
                setResponseMessage("Wiadomość została przesłana. Otwórz link z maila aby dokończyć proces.");
            }
        } catch (error: any) {
            console.error('Error in handleSubmitForm1:', error);
            switch (error.status) {
                case 400:
                    form.setFieldError('email', 'Podano nieprawidłowy adres');
                    break;
                case 404:
                    form.setFieldError('email', 'Nie znaleziono takiego adresu');
                    break;
                case 500:
                    form.setFieldError('email', 'Wystąpił wewnętrzny błąd serwera. Spróbuj ponownie później');
                    break;
                default:
                    form.setFieldError('email', 'Wystąpił nieznany błąd');
                    break;
            }
        }
    };

    return (<>
            {responseMessage ? (<Text size="md" ta="center">
                    {responseMessage}
                </Text>) : (<form onSubmit={form.onSubmit(handleSubmitForm)}>
                    <TextInput
                        size="md"
                        label="Email"
                        placeholder="mail@gatherly.com"
                        {...form.getInputProps('email')}
                        required
                    />
                    <Group justify="space-between" mt="lg">
                        <Anchor component="div" size="md" onClick={() => setAuthMethod(options.loginTraditional)}>
                            Logowanie
                        </Anchor>
                        <Anchor component="div" size="md" onClick={() => setAuthMethod(options.register)}>
                            Rejestracja
                        </Anchor>
                    </Group>
                    <Button fullWidth mt="lg" type="submit">
                        Resetuj hasło
                    </Button>
                </form>)}
        </>);
}

export default Recover;
