import React, {useEffect, useState} from "react";
import {Avatar, Button, Group, Paper, rem, Space, Text, TextInput, useMantineTheme} from "@mantine/core";
import { closeAllModals } from "@mantine/modals";
import UpdateUserData from "@/lib/widgets/Settings/UpdateUserData";
import {UserInfo} from "@/lib/interfaces/types";
import JwtTokenValid from "@/lib/auth/GetUserInfo";
import {addNotification} from "@/lib/utils/notificationsManager";

interface Errors {
    name?: string;
    email?: string;
    avatar?: string;
}

const ChangeBasicUserData = () => {
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const [avatar, setAvatar] = useState('');
    const [errors, setErrors] = useState<Errors>({});
    const [loading, setLoading] = useState(false);
    const theme = useMantineTheme();

    const avatarUrls = Array.from({ length: 15 }, (_, i) => `/avatars/avatar${i + 1}.png`);

    useEffect(() => {
        try{
            const fetchUserInfo = async () => {
                const x = await JwtTokenValid();
                if (x) {
                    setName(x.name);
                    setEmail(x.email);
                    setAvatar(x.avatarName);
                }
            };
            fetchUserInfo();
        }
        catch{
            addNotification({
                title: 'Wystąpił błąd',
                message: 'Nie udało się pobrać informacji.',
                color: 'red',
            });
        }
    }, []);

    const handleSubmit = () => {
        setLoading(true);
        const validationErrors: Errors = {};
        if (!name) validationErrors.name = "Nazwa użytkownika jest wymagana";
        if (!email) validationErrors.email = "Adres email jest wymagany";
        if (!email.match(/^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/)) validationErrors.email = "Podany adres jest nieprawidłowy";
        if (!avatar) validationErrors.avatar = "Należy wybrać avatar";
        if (Object.keys(validationErrors).length > 0) {
            setErrors(validationErrors);
            setLoading(false);
            return;
        }
        try{
            UpdateUserData({
                name: name,
                email: email,
                avatarName: avatar
            }).then(() => {
                addNotification({
                    title: 'Sukces',
                    message: 'Profil użytkownika został zaktualizowany.',
                    color: 'green',
                });
                setTimeout(() => {
                    setLoading(false);
                    window.location.reload();
                }, 3000);
            });
        }
        catch{
            addNotification({
                title: 'Wystąpił błąd',
                message: 'Profil użytkownika nie został zaktualizowany.',
                color: 'red',
            });
        }
    };

    const handleRejectChanges = () => {
        closeAllModals();
    };

    return (
        <>
            <Text size="xl" c="dimmed" fw={500}>Nazwa użytkownika</Text>
            <TextInput
                variant="unstyled"
                size="md"
                value={name}
                name="name"
                placeholder="Nowe spotkanie"
                onChange={event => {setName(event.target.value)
                    setErrors(prevErrors => ({
                    ...prevErrors,
                    name: undefined
                }));
            }}
                error={errors.name}
            />
            <Text size="xl" c="dimmed" fw={500}>Adres email</Text>
            <TextInput
                variant="unstyled"
                size="md"
                value={email}
                name="email"
                placeholder="Opis spotkania"
                onChange={event => {setEmail(event.target.value)
                    setErrors(prevErrors => ({
                        ...prevErrors,
                        email: undefined
                    }));
                }}
                error={errors.email}
            />
            <Paper>
                <Text size="xl" c="dimmed" fw={500}>Wybierz avatar</Text>
                <Space h="sm"/>
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: '20px' }}>
                    {avatarUrls.map((url, index) => (
                        <Avatar
                            key={index}
                            src={url}
                            alt={`Avatar ${index + 1}`}
                            onClick={() => setAvatar(url.substring(url.lastIndexOf('/') + 1, url.lastIndexOf('.')))}
                            size={rem(80)}
                            style={{
                                cursor: 'pointer',
                                border: ("/avatars/"+avatar+".png") === url ? `4px solid ${theme.colors.violet[5]}` : undefined
                            }}
                        />
                    ))}
                </div>
                {errors.avatar && <Text color="red">{errors.avatar}</Text>}
            </Paper>
            <Space h="md"/>
            <Group m="lg" ml="0">
            <Button onClick={handleSubmit} loading={loading}>Zatwierdź zmiany</Button>
            <Button variant="outline" color="red" onClick={handleRejectChanges} loading={loading}>Odrzuć zmiany</Button>
            </Group>
        </>
    );
};

export default ChangeBasicUserData;
