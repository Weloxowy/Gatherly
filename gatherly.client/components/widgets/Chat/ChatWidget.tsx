import React, {useEffect, useRef, useState} from 'react';
import * as signalR from "@microsoft/signalr";
import axios from "axios";
import {Avatar, Button, Group, ScrollArea, TextInput, Tooltip} from "@mantine/core";
import dayjs from "dayjs";
import classes from "./ChatWidget.module.css";
import adjustTimeToLocal from "@/lib/widgets/Meetings/adjustTimeToLocal";

const axiosInstance = axios.create({
    baseURL: "https://localhost:44329",
});

enum ErrorTypes {
    none = "none",
    lengthError = "lengthError",
    connectionError = "connectionError"
}

const ChatWidget = (meetingId: string) => {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [messages, setMessages] = useState([]);
    const [value, setValue] = useState('');
    const viewport = useRef<HTMLDivElement>(null);
    const [errorState, setErrorState] = useState<ErrorTypes>(ErrorTypes.none); // Poprawione przypisanie typu

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            //@ts-ignore
            .withUrl(`${axiosInstance.defaults.baseURL}/chathub?meetingId=${meetingId.meetingId}`, {
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
            connection.start()
                .then(() => {
                    connection.send("LoadMessageHistory");

                    connection.on("ReceiveMessage", (user, message) => {
                        //@ts-ignore
                        setMessages(prevMessages => [...prevMessages, message]);
                    });

                    connection.on("ReceiveMessageHistory", (history) => {
                        setMessages(history);

                        setErrorState(ErrorTypes.none);
                    });
                })
                .catch(err => {
                    console.error("Błąd połączenia:", err);
                    setErrorState(ErrorTypes.connectionError);
                });
        }

        return () => {
            if (connection) {
                connection.off("ReceiveMessage");
                connection.off("ReceiveMessageHistory");
                connection.stop()
                    .catch(() => {
                        setErrorState(ErrorTypes.connectionError);
                    });
            }
        };
    }, [connection]);

    const sendMessage = (content: string) => {
        if (content.length > 300) {
            setErrorState(ErrorTypes.lengthError);
            return;
        }
        if (connection && connection.state === "Connected") {
            connection.send("SendMessage", content)
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

    // @ts-ignore
    return (
        <div className={classes.chatContainer}>
            <h1>Chat</h1>
            <ScrollArea h={320} viewportRef={viewport} className={classes.scrollArea} scrollbars="y">
                <div className={classes.messagesContainer}>
                    {messages.map((msg, index) => (
                        <Group key={index}>
                            {//@ts-ignore
                                msg.typesOfMessage !== 2 ? (
                                <Tooltip
                                    withArrow
                                    //@ts-ignore
                                    label={msg.userName + " | " + dayjs(adjustTimeToLocal(msg.timestamp)).format("DD.MM HH:mm")}
                                >
                                    <Avatar src={//@ts-ignore
                                        "/avatars/" + msg.userAvatar + ".png"}></Avatar>
                                </Tooltip>
                            ) : null}
                            <p key={index} className={//@ts-ignore
                                msg.typesOfMessage === 0
                                    ? classes.loggedMessage//@ts-ignore
                                    : msg.typesOfMessage === 1
                                        ? classes.otherMessage
                                        : classes.systemMessage
                            }>
                                {//@ts-ignore
                                    msg.content}
                            </p>
                        </Group>
                    ))}
                </div>
            </ScrollArea>
            <Group align="end" className={classes.inputPanel}>
                <TextInput
                    className={classes.textInput}
                    value={value}
                    onChange={(event) => {setValue(event.currentTarget.value); setErrorState(ErrorTypes.none)}}
                    placeholder={"Wiadomość"}
                    error={errorState === ErrorTypes.lengthError ? 'Wiadomość jest zbyt długa. Limit wynosi 300 znaków.' : (errorState === ErrorTypes.connectionError ? 'Nastąpił problem z połączeniem. Odśwież kartę aby ponowić połączenie.' : '')}
                />
                <Button variant={"outline"} size={"sm"} onClick={() => sendMessage(value)}> Wyślij </Button>
            </Group>
        </div>
    );
}

export default ChatWidget;
