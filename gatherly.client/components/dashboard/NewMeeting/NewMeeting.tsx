﻿import React, { useState } from "react";
import {
    Button,
    Group,
    Text,
    TextInput,
    Tooltip,
    useMantineTheme
} from "@mantine/core";
import AddressSearchBar from "@/components/dashboard/AddressSearchBar/AddressSearchBar";
import { DateTimePicker } from "@mantine/dates";
import 'dayjs/locale/pl';
import { closeAllModals } from "@mantine/modals";
import TimezoneSelectBar from "@/components/dashboard/TimezoneSelectBar/TimezoneSelectBar";
import { IconHelpCircle } from "@tabler/icons-react";
import CreateMeeting from "@/lib/widgets/Meetings/CreateMeeting";
import '@mantine/dates/styles.css';
import dayjs from "dayjs";
import {MeetingErrors} from "@/lib/interfaces/types";
import {addNotification} from "@/lib/utils/notificationsManager";

const NewMeeting = () => {
    const [title, setTitle] = useState('');
    const [desc, setDesc] = useState('');
    const [startDate, setStartDate] = useState<Date | null>(null);
    const [endDate, setEndDate] = useState<Date | null>(null);
    const [address, setAddress] = useState({ lat: '', lon: '', name: '' });
    const [timezone, setTimezone] = useState('');
    const [errors, setErrors] = useState<MeetingErrors>({});
    const [loading, setLoading] = useState(false);
    const [checkboxChecked, setCheckboxChecked] = useState(false);
    const handleSubmit = () => {
        setLoading(true);
        const validationErrors: MeetingErrors = {};
        if (!title) validationErrors.title = "Nazwa spotkania jest wymagana";
        if (!desc) validationErrors.desc = "Opis spotkania jest wymagany";
        if (!startDate) validationErrors.startDate = "Data rozpoczęcia spotkania jest wymagana";
        if (!endDate) validationErrors.endDate = "Data zakończenia spotkania jest wymagana";
        if (!address.name) validationErrors.address = "Adres spotkania jest wymagany";
        if (!timezone) validationErrors.timezone = "Strefa czasowa jest wymagana";
        if (startDate && endDate && dayjs(startDate).unix() > dayjs(endDate).unix())
            validationErrors.startDate = "Data rozpoczęcia nie może być później niż data zakończenia";
        if (Object.keys(validationErrors).length > 0) {
            setErrors(validationErrors);
            setLoading(false);
            return;
        }
        try{
            CreateMeeting({
                meetingName: title,
                description: desc,
                startOfTheMeeting: startDate,
                endOfTheMeeting: endDate,
                placeName: address.name,
                lat: address.lat,
                lon: address.lon,
                isMeetingTimePlanned: true,
                timeZone: timezone,
            }).then(r => {
                addNotification({
                    title: 'Sukces',
                    message: 'Spotkanie zostało utworzone pomyślnie. Zaraz nastąpi przeniesienie do spotkania.',
                    color: 'green',
                });
                setTimeout(() => {
                    setLoading(false);
                    window.location.href = window.location.origin + "/meeting/" + r;
                }, 3000);
            });
        }
        catch(error){
            addNotification({
                title: 'Wystąpił błąd',
                message: 'Spotkanie nie zostało utworzone pomyślnie.',
                color: 'red',
            });
        }

    };

    const handleRejectChanges = () => {
        closeAllModals();
    };

    const theme = useMantineTheme();
    return (
        <>
            <Text size="xl" c="dimmed" fw={500}>Nazwa spotkania</Text>
            <TextInput
                value={title}
                name="title"
                placeholder="Nowe spotkanie"
                onChange={event => {setTitle(event.target.value)
                    setErrors(prevErrors => ({
                        ...prevErrors,
                        title: undefined
                    }));
                }}
                error={errors.title}
            />
            <Text size="xl" c="dimmed" fw={500}>Opis</Text>
            <TextInput
                value={desc}
                name="desc"
                placeholder="Opis spotkania"
                onChange={event => {setDesc(event.target.value)
                    setErrors(prevErrors => ({
                        ...prevErrors,
                        desc: undefined
                    }));
                }}
                error={errors.desc}
            />

            <Group>
                <Text size="xl" c="dimmed" fw={500}>Adres</Text>
                <Tooltip
                    label="Wpisz adres i kliknij enter. Jeżeli żaden z podpowiadanych nie pasuje, wybierz ostatnią opcję, która będzie wartością jaką wpisałeś.">
                    <IconHelpCircle size={16} color={theme.colors.gray[5]} />
                </Tooltip>
            </Group>
            <AddressSearchBar
                initialAddress=""
                onSelect={location => {setAddress(location)
                    setErrors(prevErrors => ({
                        ...prevErrors,
                        address: undefined
                    }));
                }}
            />
            {errors.address && <Text color="red">{errors.address}</Text>}

            <Text size="xl" c="dimmed" fw={500}>Data i godzina rozpoczęcia spotkania</Text>
            <DateTimePicker
                valueFormat="DD.MM.YYYY HH:mm"
                value={startDate}
                placeholder="Wybierz datę"
                locale='pl'
                onChange={newDate => {setStartDate(newDate)
                    setErrors(prevErrors => ({
                        ...prevErrors,
                        startDate: undefined
                    }));
                }}
                name="startDate"
                error={errors.startDate}
            />
            <Text size="xl" c="dimmed" fw={500}>Data i godzina zakończenia spotkania</Text>
            <DateTimePicker
                valueFormat="DD.MM.YYYY HH:mm"
                value={endDate}
                placeholder="Wybierz datę"
                locale='pl'
                name="endDate"
                disabled={checkboxChecked}
                onChange={newDate => {setEndDate(newDate)
                    setErrors(prevErrors => ({
                        ...prevErrors,
                        endDate: undefined
                    }));
                }}
                error={errors.endDate}
            />
            <Text size="xl" c="dimmed" fw={500}>Strefa czasowa spotkania</Text>
            <TimezoneSelectBar
                onSelect={(timezone) => {
                    setTimezone(timezone);
                    setErrors(prevErrors => ({
                        ...prevErrors,
                        timezone: undefined
                    }));
                }}
            />
            {errors.timezone && <Text c="red">{errors.timezone}</Text>}
            <Group style={{
                alignItems: "flex-start",
                alignContent: "center",
                justifyContent: "space-between",
                padding: "10px"
            }}>
                <Group m="lg" ml="0">
                <Button onClick={handleSubmit} loading={loading} >Zatwierdź zmiany</Button>
                <Button variant="outline" loading={loading} color="red" onClick={handleRejectChanges} >Odrzuć zmiany</Button>
                </Group>
            </Group>
        </>
    );
};

export default NewMeeting;
