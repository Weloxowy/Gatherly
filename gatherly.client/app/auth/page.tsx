"use client"
import {Anchor, Container, Paper, Text, Title} from '@mantine/core';
import {useState} from "react";
import LoginTraditional from "@/components/auth/LoginTraditional";
import LoginByCode from "@/components/auth/LoginByCode";
import Register from "@/components/auth/Register";
import Recover from "@/components/auth/Recover";
import {authOptions} from "@/lib/interfaces/types";

export default function Auth() {
    const [authMethod, setAuthMethod] = useState(authOptions.loginTraditional);

    return (
        <main>
            <Container size="sm" my="xl" pt={100}>
                <Container ta="center" style={{alignItems: "center"}}>
                    <Title order={1} ta="center">
                        Witamy!
                    </Title>
                    <Text c="dimmed" size="md" ta="center" mt={5}>
                        {authMethod === authOptions.loginTraditional && (
                            <>
                                <>
                                    Wpisz poniżej adres mailowy aby się zalogować.
                                </>
                                <Anchor component="button" size="md"
                                        onClick={() => setAuthMethod(authOptions.register)}>
                                    Nie posiadasz konta? Zarejestruj się
                                </Anchor>
                            </>
                        )
                        }
                        {authMethod === authOptions.loginByCode && (
                            <>
                                <>
                                    Wpisz poniżej adres mailowy aby się zalogować poprzez usługę jednorazowego
                                    logowania.
                                </>
                                <Anchor component="button" size="md"
                                        onClick={() => setAuthMethod(authOptions.register)}>
                                    Nie posiadasz konta? Zarejestruj się
                                </Anchor>
                            </>
                        )
                        }
                        {authMethod === authOptions.register && (
                            <>
                                <>
                                    Zarejestruj się aby móc korzystać z usług.
                                </>
                                <Anchor component="button" size="md"
                                        onClick={() => setAuthMethod(authOptions.loginTraditional)}>
                                    Posiadasz już konto? Zaloguj się
                                </Anchor>
                            </>
                        )
                        }
                        {authMethod === authOptions.recover && (
                            <>
                                <>
                                    Wpisz poniżej adres email aby zresetować hasło.
                                </>
                                <Anchor component="button" size="md"
                                        onClick={() => setAuthMethod(authOptions.loginTraditional)}>
                                    Posiadasz już konto? Zaloguj się
                                </Anchor>
                            </>
                        )
                        }
                    </Text>
                </Container>
                <Paper withBorder shadow="md" p="lg" mt="xl" radius="md">
                    {authMethod === authOptions.loginTraditional &&
                        <LoginTraditional setAuthMethod={setAuthMethod} options={authOptions}/>}
                    {authMethod === authOptions.loginByCode &&
                        <LoginByCode setAuthMethod={setAuthMethod} options={authOptions}/>}
                    {authMethod === authOptions.register &&
                        <Register setAuthMethod={setAuthMethod} options={authOptions}/>}
                    {authMethod === authOptions.recover &&
                        <Recover setAuthMethod={setAuthMethod} options={authOptions}/>}
                </Paper>
            </Container>
        </main>
    );
}
