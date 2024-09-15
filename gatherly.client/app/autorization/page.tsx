"use client"
import { Button, Container, Title } from '@mantine/core';
import logoutUser from "@/lib/auth/logoutUser";
import axiosInstance from "@/lib/utils/AxiosInstance";
import Link from "next/link";
//page only for tests
export default function AutorizationTestPage() {
    //const [data, setData] = useState<string | null>(null);
    //const [user, setUser] = useState<string | null>(null);
/*
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
*/
    const handleLogout = async () => {
        try {
            await logoutUser();
            window.location.href = '/auth';
        } catch (error) {
            console.error('Logout failed:', error);
        }
    };

    const handleGetInfo = async () => {
        try {
            const response = await axiosInstance.get('User/profile', {
            });
        } catch (error) {
            console.error('Get user info failed:', error);
        }
    };
    const handleRefreshValidation = async () => {
        try {
            const response = await axiosInstance.post(
                'Tokens/refresh/validate',
                {}
            );
        } catch (error) {
            console.error('Refresh validation failed:', error);
        }
    };

    const handleGetInfoById = async () => {
        try {
            const response = await axiosInstance.get('User/profile/7CB79F46-55D5-4908-BE27-B1C3010D408F', {
            });
        } catch (error) {
            console.error('Get user info failed:', error);
        }
    };

    const handleJwtValidation = async () => {
        try {
            const response = await axiosInstance.post(
                'Tokens/jwt/validate',
                {},
            );
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
                        {/*
                            data !== null ?
                                (
                                    `${data}\n${user}`
                                ) :
                                ("Użytkownik wylogowany")
                                */
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
                    <Button onClick={handleGetInfoById}>
                        GET info by id
                    </Button>
                    <Button component={Link} href={"/home"}>
                        Przejdź do panelu
                    </Button>
                </Container>
            </Container>
        </main>
    );
}
