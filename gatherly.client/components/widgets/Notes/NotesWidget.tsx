import React, { useEffect, useState, useRef } from "react";
import { Button, Checkbox, Group, ScrollArea, TextInput, Transition } from "@mantine/core";
import classes from './NotesWidget.module.css';
import NotesGet from "@/lib/widgets/Notes/NotesGet";
import NotesPost from "@/lib/widgets/Notes/NotesPost";
import {addNotification} from "@/lib/utils/notificationsManager";
interface Note {
    Id: string;
    Text: string;
    Checked?: boolean;
}

const NotesWidget = () => {
    const [notes, setNotes] = useState<Note[]>([]);
    const [newNoteText, setNewNoteText] = useState<string>("");
    const notesRef = useRef<Note[]>(notes);

    useEffect(() => {
        const fetchNotes = async () => {
            try {
                const response = await NotesGet();
                if (response && response.Notes && Array.isArray(response.Notes.Note)) {
                    const fetchedNotes = response.Notes.Note.map((note: any) => ({
                        Id: note.Id,
                        Text: note.Text,
                        Checked: note.Checked ?? false
                    }));
                    setNotes(fetchedNotes);
                    notesRef.current = fetchedNotes;
                } else {
                    addNotification({
                        title: 'Wystąpił błąd',
                        message: 'Wystąpił błąd z notatkami. Skontaktuj się z administratorem w celu naprawy błędu.',
                        color: 'red',
                    });
                    //console.error("Niepoprawny format danych", response);
                }
            } catch (error) {
                addNotification({
                    title: 'Wystąpił błąd',
                    message: 'Zmiany nie zostały zapisane pomyślnie.',
                    color: 'red',
                });
                //console.error("Failed to get notes for logged user", error);
            }
        };

        fetchNotes();
    }, []);

    useEffect(() => {
        const sendNotes = async () => {
            try {
                const jsonData = JSON.stringify({
                    Notes: {
                        Note: notesRef.current.map(({ Id, Text, Checked }) => ({ Id, Text, Checked }))
                    }
                });

                //console.log("Sending JSON data to backend:", jsonData);
                await NotesPost(jsonData);
            } catch (error) {
                addNotification({
                    title: 'Wystąpił błąd',
                    message: 'Notatki nie zostały przesłane.',
                    color: 'red',
                });
                //console.error("Failed to put notes for logged user", error);
            }
        };
        return () => {
            sendNotes();
        };
    }, [notes]);

    const handleAdd = () => {
        if (newNoteText.trim() !== "") {
            const newId = (notes.length > 0 ? (parseInt(notes[notes.length - 1].Id) + 1).toString() : "1");

            setNotes((prevNotes) => {
                const updatedNotes = [
                    ...prevNotes,
                    { Id: newId, Text: newNoteText, Checked: false },
                ];
                notesRef.current = updatedNotes;
                return updatedNotes;
            });
            setNewNoteText("");
        }
    };

    const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
        if (event.key === 'Enter') {
            handleAdd();
        }
    };

    return (
        <>
            <div className={classes.list}>
                <ScrollArea>
                    {notes.map((note) => (
                        <Transition key={note.Id} mounted={!note.Checked} transition="fade" duration={1200} timingFunction="ease">
                            {(styles) => (
                                <Group style={styles} className={classes.reminder}>
                                    <Checkbox
                                        radius={"xl"}
                                        checked={note.Checked}
                                        onChange={() => {
                                            setNotes(prevNotes => {
                                                const updatedNotes = prevNotes.map(n =>
                                                    n.Id === note.Id ? { ...n, Checked: true } : n
                                                );

                                                notesRef.current = updatedNotes;
                                                return updatedNotes;
                                            });
                                            setTimeout(() => {
                                                setNotes(prevNotes => {
                                                    const updatedNotes = prevNotes.filter(n => n.Id !== note.Id);

                                                    notesRef.current = updatedNotes;
                                                    return updatedNotes;
                                                });
                                            }, 2000);
                                        }}
                                        label={note.Text}
                                        styles={{ label: { textDecoration: note.Checked ? 'line-through' : 'none' } }}
                                    />
                                </Group>
                            )}
                        </Transition>
                    ))}
                </ScrollArea>
            </div>
            <div className={classes.inputGroup}>
                <TextInput
                    variant="unstyled"
                    placeholder="Nowa notatka"
                    value={newNoteText}
                    onKeyDown={handleKeyDown}
                    onChange={(event) => setNewNoteText(event.currentTarget.value)}
                    className={classes.input}
                />
                <Button onClick={handleAdd}>Dodaj</Button>
            </div>
        </>
    );
};

export default NotesWidget;
