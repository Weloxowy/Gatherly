import React, { useEffect, useRef, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import axios from 'axios';
import { Avatar, Button, Group, ScrollArea, TextInput, Tooltip } from '@mantine/core';
import dayjs from 'dayjs';
import classes from './ChatWidget.module.css';
import adjustTimeToLocal from '@/lib/widgets/Meetings/adjustTimeToLocal';
import { IconSend } from '@tabler/icons-react';
import { Person } from '@/lib/interfaces/types';

const axiosInstance = axios.create({
    baseURL: process.env.CHAT_ADDRESS,
});

enum ErrorTypes {
    none = 'none',
    lengthError = 'lengthError',
    connectionError = 'connectionError',
}

interface Message {
    userName: string;
    content: string;
    timestamp: string;
    userAvatar: string;
    typesOfMessage: number;
}

const ChatWidget = ({ meetingId, usersList }: { meetingId: string; usersList: Person[] }) => {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [messages, setMessages] = useState<Message[]>([]);
    const [value, setValue] = useState('');
    const viewport = useRef<HTMLDivElement>(null);
    const [errorState, setErrorState] = useState<ErrorTypes>(ErrorTypes.none);

    const [suggestions, setSuggestions] = useState<Person[]>([]); // Lista sugestii użytkowników
    const [showAutocomplete, setShowAutocomplete] = useState(false); // Wyświetlanie listy autouzupełniania
    const [cursorPos, setCursorPos] = useState(0); // Pozycja kursora dla zastąpienia tekstu

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const inputValue = event.currentTarget.value;
        setValue(inputValue);
        setErrorState(ErrorTypes.none);

        const atPos = inputValue.lastIndexOf('@');
        setCursorPos(atPos);

        if (atPos !== -1) {
            const searchText = inputValue.slice(atPos + 1); // Tekst po znaku `@`
            if (searchText.length > 0) {
                setSuggestions(
                    usersList.filter((user) =>
                        user.name.toLowerCase().includes(searchText.toLowerCase())
                    )
                );
                setShowAutocomplete(true);
            } else {
                setShowAutocomplete(false);
            }
        } else {
            setShowAutocomplete(false);
        }
    };

    const handleSelectUser = (user: Person) => {
        // Zastąp tekst po znaku `@` wybranym użytkownikiem
        const textBeforeAt = value.slice(0, cursorPos);
        setValue(`${textBeforeAt}@${user.name} `); // Dodaj nazwę użytkownika do wiadomości
        setShowAutocomplete(false);
    };

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${axiosInstance.defaults.baseURL}/chathub?meetingId=${meetingId}`, {
                withCredentials: true,
            })
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);
    }, [meetingId]);

    const scrollToBottom = () =>
        viewport.current!.scrollTo({ top: viewport.current!.scrollHeight, behavior: 'smooth' });

    useEffect(() => {
        scrollToBottom();
    }, [messages]);

    useEffect(() => {
        if (connection) {
            connection
                .start()
                .then(() => {
                    connection.send('LoadMessageHistory');

                    connection.on('ReceiveMessage', (user: string, message: Message) => {
                        setMessages((prevMessages) => [...prevMessages, message]);
                    });

                    connection.on('ReceiveMessageHistory', (history: Message[]) => {
                        setMessages(history);
                        setErrorState(ErrorTypes.none);
                    });
                })
                .catch((err) => {
                    console.error('Błąd połączenia:', err);
                    setErrorState(ErrorTypes.connectionError);
                });
        }

        return () => {
            if (connection) {
                connection.off('ReceiveMessage');
                connection.off('ReceiveMessageHistory');
                connection
                    .stop()
                    .catch(() => {
                        setErrorState(ErrorTypes.connectionError);
                    });
            }
        };
    }, [connection]);

    const sendMessage = (content: string) => {
        if (content.length == 0) {
            return;
        }
        if (content.length > 300) {
            setErrorState(ErrorTypes.lengthError);
            return;
        }
        if (connection && connection.state === 'Connected') {
            connection
                .send('SendMessage', content)
                .then(() => {
                    setValue('');
                    setErrorState(ErrorTypes.none); // Wyczyszczenie błędu po udanym wysłaniu
                })
                .catch(() => {
                    setErrorState(ErrorTypes.connectionError);
                });
        } else {
            setErrorState(ErrorTypes.connectionError);
        }
    };

    const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
        if (event.key === 'Enter') {
            sendMessage(value);
        }
    };

    return (
        <div className={classes.chatContainer}>
            <h1>Chat</h1>
            <ScrollArea h={320} viewportRef={viewport} className={classes.scrollArea} scrollbars="y">
                <div className={classes.messagesContainer}>
                    {messages.map((msg, index) => (
                        <Group key={index}>
                            {msg.typesOfMessage !== 2 ? (
                                <Tooltip
                                    withArrow
                                    label={`${msg.userName} | ${dayjs(msg.timestamp).format(
                                        'DD.MM HH:mm UTC'
                                    )}`}
                                >
                                    <Avatar src={`/avatars/${msg.userAvatar}.png`} />
                                </Tooltip>
                            ) : null}
                            <p
                                key={index}
                                className={
                                    msg.typesOfMessage === 0
                                        ? classes.loggedMessage
                                        : msg.typesOfMessage === 1
                                            ? classes.otherMessage
                                            : classes.systemMessage
                                }
                            >
                                {msg.content}
                            </p>
                        </Group>
                    ))}
                </div>
            </ScrollArea>
            {showAutocomplete && (
                <div className={classes.autocomplete}>
                    {suggestions.map((user, index) => (
                        <div
                            key={index}
                            className={classes.autocompleteItem}
                            onClick={() => handleSelectUser(user)}
                        >
                            {user.name}
                        </div>
                    ))}
                </div>
            )}
            <Group align="end" className={classes.inputPanel}>
                <TextInput
                    className={classes.textInput}
                    value={value}
                    onChange={handleInputChange}
                    onKeyDown={handleKeyDown}
                    placeholder="Wiadomość"
                    error={
                        errorState === ErrorTypes.lengthError
                            ? 'Wiadomość jest zbyt długa. Limit wynosi 300 znaków.'
                            : errorState === ErrorTypes.connectionError
                                ? 'Nastąpił problem z połączeniem. Odśwież kartę, aby ponowić połączenie.'
                                : ''
                    }
                />
                <Button variant="outline" size="sm" onClick={() => sendMessage(value)}>
                    <IconSend />
                </Button>
            </Group>
        </div>
    );
};

export default ChatWidget;
