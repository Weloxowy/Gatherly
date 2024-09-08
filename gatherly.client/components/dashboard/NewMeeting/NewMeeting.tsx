import React, { useState } from "react";
import {
    Button,
    Flex,
    Group,
    Text,
    TextInput,
    Title,
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

// Define the type for errors
interface Errors {
    title?: string;
    desc?: string;
    startDate?: string;
    endDate?: string;
    address?: string;
    timezone?: string;
}

const NewMeeting = () => {
    const [title, setTitle] = useState('');
    const [desc, setDesc] = useState('');
    const [startDate, setStartDate] = useState<Date | null>(null);
    const [endDate, setEndDate] = useState<Date | null>(null);
    const [address, setAddress] = useState({ lat: '', lon: '', name: '' });
    const [timezone, setTimezone] = useState('');
    const [errors, setErrors] = useState<Errors>({}); // Use the Errors type here
    const [loading, setLoading] = useState(false);  // State for button loading

    const handleSubmit = () => {
        const validationErrors: Errors = {};

        // Simple validation logic
        if (!title) validationErrors.title = "Nazwa spotkania jest wymagana";
        if (!desc) validationErrors.desc = "Opis spotkania jest wymagany";
        if (!startDate) validationErrors.startDate = "Data rozpoczęcia spotkania jest wymagana";
        if (!endDate) validationErrors.endDate = "Data zakończenia spotkania jest wymagana";
        if (!address.name) validationErrors.address = "Adres spotkania jest wymagany";
        if (!timezone) validationErrors.timezone = "Strefa czasowa jest wymagana";
        if (startDate && endDate && dayjs(startDate).unix() > dayjs(endDate).unix())
            validationErrors.startDate = "Data rozpoczęcia nie może być później niż data zakończenia";
        if (Object.keys(validationErrors).length > 0) {
            // Set the errors in state so they can be displayed
            setErrors(validationErrors);
            return;
        }

        // If there are no validation errors, proceed with the submission
        CreateMeeting({
            meetingName: title,
            description: desc,
            startOfTheMeeting: startDate,
            endOfTheMeeting: endDate,
            placeName: address.name,
            lat: address.lon,
            lon: address.lat,
            isMeetingTimePlanned: true,
            timeZone: timezone,
        }).then(r => {
            // Simulate delay of 3 seconds before redirecting
            setTimeout(() => {
                setLoading(false); // Stop loading
                window.location.href = "https://localhost:3000/meeting/" + r;
            }, 3000);
        });
    };

    const handleRejectChanges = () => {
        closeAllModals();
    };

    const theme = useMantineTheme();

    return (
        <>
            <Text size="xl" c="dimmed" fw={500}>Nazwa spotkania</Text>
            <TextInput
                variant="unstyled"
                size="md"
                value={title}
                name="title"
                placeholder="Nowe spotkanie"
                onChange={event => setTitle(event.target.value)}
                error={errors.title}
            />
            {errors.title && <Text color="red">{errors.title}</Text>}

            <Text size="xl" c="dimmed" fw={500}>Opis</Text>
            <TextInput
                variant="unstyled"
                size="md"
                value={desc}
                name="desc"
                placeholder="Opis spotkania"
                onChange={event => setDesc(event.target.value)}
                error={errors.desc}
            />
            {errors.desc && <Text color="red">{errors.desc}</Text>}

            <Group>
                <Text size="xl" c="dimmed" fw={500}>Adres</Text>
                <Tooltip
                    label="Wpisz adres i kliknij enter. Jeżeli żaden z podpowiadanych nie pasuje, wybierz ostatnią opcję, która będzie wartością jaką wpisałeś.">
                    <IconHelpCircle size={16} color={theme.colors.gray[5]} />
                </Tooltip>
            </Group>
            <AddressSearchBar
                initialAddress=""
                onSelect={(location) => setAddress(location)}
            />
            {errors.address && <Text color="red">{errors.address}</Text>}

            <Text size="xl" c="dimmed" fw={500}>Data i godzina rozpoczęcia spotkania</Text>
            <DateTimePicker
                valueFormat="DD.MM.YYYY HH:mm"
                value={startDate}
                placeholder="Wybierz datę"
                locale='pl'
                onChange={setStartDate}
                name="startDate"
            />
            {errors.startDate && <Text color="red">{errors.startDate}</Text>}

            <Text size="xl" c="dimmed" fw={500}>Data i godzina zakończenia spotkania</Text>
            <DateTimePicker
                valueFormat="DD.MM.YYYY HH:mm"
                value={endDate}
                placeholder="Wybierz datę"
                locale='pl'
                name="endDate"
                onChange={setEndDate}
            />
            {errors.endDate && <Text color="red">{errors.endDate}</Text>}

            <Text size="xl" c="dimmed" fw={500}>Strefa czasowa spotkania</Text>
            <TimezoneSelectBar onSelect={setTimezone} />
            {errors.timezone && <Text color="red">{errors.timezone}</Text>}

            <Group style={{
                alignItems: "flex-start",
                alignContent: "center",
                justifyContent: "space-between",
                padding: "10px"
            }}>
                <Button onClick={handleSubmit} loading={loading} >Zatwierdź zmiany</Button>
                <Button variant="outline" color="red" onClick={handleRejectChanges} loading={loading}>Odrzuć zmiany</Button>
            </Group>
        </>
    );
};

export default NewMeeting;
