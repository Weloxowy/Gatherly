"use client"
import {
    TextInput,
    Checkbox,
    Anchor,
    Paper,
    Title,
    Text,
    Container,
    Group,
    Button, rem,
} from '@mantine/core';
import { useState } from "react";
import LoginTraditional from "@/components/auth/LoginTraditional";
import LoginByCode from "@/components/auth/LoginByCode";
import Register from "@/components/auth/Register";
import Recover from "@/components/auth/Recover";
import GradientBackground from "@/components/app/GradientBackground";

const options = {
    loginTraditional: 'loginTraditional',
    loginByCode: 'loginByCode',
    register: 'register',
    recover: 'recover'
};

export default function Auth() {
    const [authMethod, setAuthMethod] = useState(options.loginTraditional);

    return (
        <main>
            <Container size="sm" my="xl" pt={100}>
                <Container ta="center" style={{alignItems: "center"}}>
                    <Title order={1} ta="center">
                        Witamy!
                    </Title>
                    <Text c="dimmed" size="md" ta="center" mt={5}>
                        {authMethod === options.loginTraditional && (
                            <>
                            <>
                                Wpisz poniżej adres mailowy aby się zalogować.
                            </>
                            <Anchor component="button" size="md" onClick={() => setAuthMethod(options.register)}>
                                Nie posiadasz konta? Zarejestruj się
                            </Anchor>
                            </>
                        )
                        }
                        {authMethod === options.loginByCode && (
                        <>
                            <>
                                Wpisz poniżej adres mailowy aby się zalogować poprzez usługę jednorazowego logowania.
                            </>
                            <Anchor component="button" size="md" onClick={() => setAuthMethod(options.register)}>
                                Nie posiadasz konta? Zarejestruj się
                            </Anchor>
                        </>
                        )
                        }
                        {authMethod === options.register && (
                            <>
                                <>
                                    Zarejestruj się aby móc korzystać z usług.
                                </>
                                <Anchor component="button" size="md" onClick={() => setAuthMethod(options.loginTraditional)}>
                                    Posiadasz już konto? Zaloguj się
                                </Anchor>
                            </>
                        )
                        }
                        {authMethod === options.recover && (
                            <>
                                <>
                                    Wpisz poniżej adres email aby zresetować hasło.
                                </>
                                <Anchor component="button" size="md" onClick={() => setAuthMethod(options.loginTraditional)}>
                                    Posiadasz już konto? Zaloguj się
                                </Anchor>
                            </>
                        )
                        }
                    </Text>
                </Container>
                <Paper withBorder shadow="md" p="lg" mt="xl" radius="md">
                    {authMethod === options.loginTraditional && <LoginTraditional setAuthMethod={setAuthMethod} options={options} />}
                    {authMethod === options.loginByCode && <LoginByCode setAuthMethod={setAuthMethod} options={options} />}
                    {authMethod === options.register && <Register setAuthMethod={setAuthMethod} options={options} />}
                    {authMethod === options.recover && <Recover setAuthMethod={setAuthMethod} options={options} />}
                </Paper>
            </Container>
        </main>
    );
}
