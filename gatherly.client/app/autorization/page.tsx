"use client"
import { Button, Container, Title } from '@mantine/core';
import { useEffect, useState } from "react";
import { readFromLocalStorage } from "@/lib/auth/headers/readFromLocalStorage";
import getData from "@/lib/getData";
import logoutUser from "@/lib/auth/logoutUser";
import axiosInstance from "@/lib/AxiosInstance";
//page only for tests
export default function AutorizationTestPage() {
    const [data, setData] = useState<string | null>(null);
    const [user, setUser] = useState<string | null>(null);

    useEffect(() => {
        const storedData = readFromLocalStorage("Authorization");
        setData(storedData);
    }, []);

    useEffect(() => {
        const fetchUserData = async () => {
            const storedData = await getData();
            setUser(storedData);
        };

        fetchUserData();
    }, []);

    const handleLogout = async () => {
        try {
            await logoutUser();
            window.location.href = '/auth'; // lub inna ścieżka
        } catch (error) {
            console.error('Logout failed:', error);
        }
    };

    const handleGetInfo = async () => {
        try {
            const authorizationToken = readFromLocalStorage("Authorization");
            const refreshToken = readFromLocalStorage("Refresh");
            console.log(refreshToken);
            if (!authorizationToken || !refreshToken) {
                throw new Error("Authorization or refresh token not found");
            }
            const response = await axiosInstance.get(
                'auth/profile',
                {
                    headers: {
                        'Authorization': authorizationToken,
                        'Refresh': refreshToken
                    }
                }
            );
            console.log('Get user info response:', response);
        } catch (error) {
            console.error('Get user info failed:', error);
        }
    };

    const handleRefreshValidation = async () => {
        try {
            const authorizationToken = readFromLocalStorage("Authorization");
            const refreshToken = readFromLocalStorage("Refresh");
            console.log(refreshToken);
            if (!authorizationToken || !refreshToken) {
                throw new Error("Authorization or refresh token not found");
            }
            const response = await axiosInstance.post(
                'Tokens/refresh/validate',
                {},
                {
                    headers: {
                        'Authorization': authorizationToken,
                        'Refresh': refreshToken
                    }
                }
            );
            console.log('Refresh validation response:', response);
        } catch (error) {
            console.error('Refresh validation failed:', error);
        }
    };

    const handleJwtValidation = async () => {
        try {
            const authorizationToken = readFromLocalStorage("Authorization");
            const refreshToken = readFromLocalStorage("Refresh");
            if (!authorizationToken || !refreshToken) {
                throw new Error("Authorization or refresh token not found");
            }
            const response = await axiosInstance.post(
                'Tokens/jwt/validate',
                {},
                {
                    headers: {
                        'Authorization': authorizationToken,
                        'Refresh': refreshToken
                    }
                }
            );
            console.log('JWT validation response:', response);
        } catch (error) {
            console.error('JWT validation failed:', error);
        }
    };

    return (
        <main>
            <Container size="sm" my="xl">
                <Container>
                    <Title order={1} ta="center">
                        Witamy!
                    </Title>
                    <>
                        {
                            data !== null ?
                                (
                                    `${data}\n${user}`
                                ) :
                                ("Użytkownik wylogowany")
                        }
                    </>
                    <Button onClick={handleLogout}>
                        Wyloguj się
                    </Button>
                    <Button onClick={handleRefreshValidation}>
                        Walidacja refresh
                    </Button>
                    <Button onClick={handleJwtValidation}>
                        Walidacja jwt
                    </Button>
                    <Button onClick={handleGetInfo}>
                        GET info
                    </Button>
                </Container>
            </Container>
        </main>
    );
}
