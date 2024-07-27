import React, { useState } from "react";
import { Button, Checkbox, Group, ScrollArea, TextInput, Transition } from "@mantine/core";
import classes from './NotesWidget.module.css';

const initialData = [
    { id: 0, text: 'Kupić wino na spotkanie czwartkowe', checked: false },
    { id: 1, text: 'Przelać Ani 20 zł', checked: false },
    { id: 2, text: 'Hasło: 203344132141', checked: false },
    { id: 3, text: '17:50 taksówka', checked: false },
    { id: 4, text: 'Kupić wino na spotkanie', checked: false },
];

const NotesWidget = () => {
    const [reminders, setReminders] = useState(initialData);
    const [newReminder, setNewReminder] = useState("");
    const [deleting, setDeleting] = useState([]);

    const handleDelete = (id : number) => {
        setReminders((prevReminders) =>
            prevReminders.map((r) =>
                r.id === id ? { ...r, checked: !r.checked } : r
            )
        );
        setDeleting((prevDeleting) => [...prevDeleting, id]);
        setTimeout(() => {
            setReminders((prevReminders) => prevReminders.filter((reminder) => reminder.id !== id));
            setDeleting((prevDeleting) => prevDeleting.filter((deletingId) => deletingId !== id));
        }, 2000); // Duration of the fade transition
    };

    const handleAdd = () => {
        if (newReminder.trim() !== "") {
            const newId = (reminders.length > 0 ? Math.max(...reminders.map(r => r.id)) + 2 : 0).toString();
            setReminders((prevReminders) => [
                ...prevReminders,
                { id: newId, text: newReminder, checked: false },
            ]);
            setNewReminder("");
        }
    };

    const handleKeyDown = (event) => {
        if (event.key === 'Enter') {
            handleAdd();
        }
    };

    const renderReminder = (reminder) => (
        <Transition key={reminder.id} mounted={!deleting.includes(reminder.id)} transition="fade"
                    duration={1200} timingFunction="ease">
            {(styles) => (
                <ScrollArea.Autosize>
                    <Group style={styles} position="apart" className={classes.reminder}>
                        <Checkbox
                            radius={"xl"}
                            checked={reminder.checked}
                            onChange={() => handleDelete(reminder.id)}
                            label={reminder.text}
                            styles={{ label: { textDecoration: reminder.checked ? 'line-through' : 'none' } }}
                        />
                    </Group>
                </ScrollArea.Autosize>
            )}
        </Transition>
    );

    return (
        <>
            <div className={classes.list}>
                <ScrollArea>
                    {reminders.map(renderReminder)}
                </ScrollArea>
            </div>
            <div className={classes.inputGroup}>
                <TextInput
                    variant="unstyled"
                    placeholder="Nowe przypomnienie"
                    value={newReminder}
                    onKeyDown={handleKeyDown}
                    onChange={(event) => setNewReminder(event.currentTarget.value)}
                    className={classes.input}
                />
                <Button onClick={handleAdd}>Dodaj</Button>
            </div>
        </>
    );
};

export default NotesWidget;
