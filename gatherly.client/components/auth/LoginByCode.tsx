﻿import React, {useState} from "react";
import {Anchor, Button, Group, PinInput, TextInput} from "@mantine/core";
import {useForm} from "@mantine/form";
import sendSsoCode from "../../lib/auth/sendSsoCode";
import sendReturnedCode from "../../lib/auth/sendReturnedCode";
import {AuthProps} from "@/lib/interfaces/types";

const LoginByCode: React.FC<AuthProps> = ({setAuthMethod, options}) => {
    const [loading, setLoading] = useState(false);
    const [step, setStep] = useState<number>(1);
    const [email, setEmail] = useState('');
    const form1 = useForm({
        initialValues: {
            email: '',
        }, validate: {
            email: (value) => (/^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/.test(value) ? null : 'Nieprawidłowy adres email'),
        },
    });

    const form2 = useForm({
        initialValues: {
            code: '',
        }, validate: {
            code: (value) => (value.length == 6 ? null : 'Kod powinien mieć 6 cyfr'),
        },
    });

    const handleSubmitForm1 = async (values: { email: string }) => {
        setLoading(true);
        try {
            const res = await sendSsoCode(values.email);
            setEmail(values.email);
            setStep(2);
        } catch (error: any) {
            console.error('Error in handleSubmitForm1:', error);
            switch (error.status) {
                case 400:
                    form1.setFieldError('email', 'Podano nieprawidłowy adres');
                    break;
                case 404:
                    form1.setFieldError('email', 'Nie znaleziono takiego adresu');
                    break;
                case 500:
                    form1.setFieldError('email', 'Wystąpił wewnętrzny błąd serwera. Spróbuj ponownie później');
                    break;
                default:
                    form1.setFieldError('email', 'Wystąpił nieznany błąd');
                    break;
            }
        }
        finally {
            setLoading(false);
        }
    };


    const handleSubmitForm2 = async (values: { code: string }) => {
        setLoading(true);
        try {
            await sendReturnedCode(email, values.code);
            window.location.href = "/home";
        } catch (error: any) {
            console.error('Error in handleSubmitForm2:', error);
            switch (error.code) {
                case 400:
                    form2.setFieldError('code', 'Podano nieprawidłowy adres');
                    break;
                case 404:
                    form2.setFieldError('code', 'Błędny kod. Spróbuj ponownie');
                    break;
                case 500:
                    form2.setFieldError('code', 'Wystąpił wewnętrzny błąd serwera. Spróbuj ponownie później');
                    break;
                default:
                    form2.setFieldError('code', 'Wystąpił nieznany błąd');
                    break;
            }
        }
        finally {
            setLoading(false);
        }
    };


    return (<>
            {step === 1 ? (<form onSubmit={form1.onSubmit(handleSubmitForm1)}>
                    <TextInput size="md" label="Email" key={form1.key('email')}
                               {...form1.getInputProps('email')}
                               placeholder="mail@gatherly.com" required/>

                    <Group justify="space-between" mt="lg">
                        <Anchor component="div" size="md" onClick={() => setAuthMethod(options.loginTraditional)}>
                            Logowanie tradycyjne
                        </Anchor>
                        <Anchor component="div" size="md" onClick={() => setAuthMethod(options.recover)}>
                            Odzyskaj konto
                        </Anchor>
                    </Group>
                    <Button loading={loading} fullWidth mt="lg" type="submit">
                        Wyślij kod
                    </Button>
                </form>) : (<form onSubmit={form2.onSubmit(handleSubmitForm2)}>
                <div style={{display: 'flex', justifyContent: 'center'}}>
                    <PinInput size="md" key={form2.key('code')}
                              {...form2.getInputProps('code')} length={6} mask type="number" inputType="number"
                              inputMode="numeric"
                    />
                </div>
                    <Button loading={loading} fullWidth mt="lg" type="submit">
                        Zatwierdź kod
                    </Button>
            </form>)}
    </>);
};

export default LoginByCode;
