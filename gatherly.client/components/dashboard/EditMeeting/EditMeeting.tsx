'use client';
import React, { useState } from "react";
import { Button, Group, Text, TextInput, Tooltip, useMantineTheme } from "@mantine/core";
import { ExtendedMeeting } from "@/lib/interfaces/types";
import AddressSearchBar from "@/components/dashboard/AddressSearchBar/AddressSearchBar";
import { DateTimePicker } from "@mantine/dates";
import 'dayjs/locale/pl';
import { closeAllModals } from "@mantine/modals";
import TimezoneSelectBar from "@/components/dashboard/TimezoneSelectBar/TimezoneSelectBar";
import { IconHelpCircle } from "@tabler/icons-react";
import PatchMeeting from "@/lib/widgets/Meetings/PatchMeeting";
import getPageName from "@/lib/utils/qrGenerator/getPageName";
import adjustTimeToLocal from "@/lib/widgets/Meetings/adjustTimeToLocal";
import dayjs from "dayjs";

// Helper function to calculate the offset in minutes
const getTimezoneOffset = (date: Date) => {
    return date.getTimezoneOffset(); // Returns the difference in minutes from UTC
}

interface MeetingDetailsProps {
    data: ExtendedMeeting;
}

const EditMeeting: React.FC<MeetingDetailsProps> = ({ data }) => {
    const [title, setTitle] = useState(data.name);
    const [desc, setDesc] = useState(data.desc);
    const [startDate, setStartDate] = useState<Date | null>(dayjs(adjustTimeToLocal(data.startOfTheMeeting,data.timezoneOffset)).toDate());
    const [endDate, setEndDate] = useState<Date | null>(dayjs(adjustTimeToLocal(data.endOfTheMeeting,data.timezoneOffset)).toDate());
    const [address, setAddress] = useState({ lat: data.lat, lon: data.lon, name: data.placeName });
    const [timezone, setTimezone] = useState<string | undefined>();
    const link = getPageName(window.location.href)?.toString();

    const handleSubmit = () => {
        if (startDate && endDate) {
            // Get the offset in minutes
            const offset = getTimezoneOffset(startDate);

            // Convert dates to ISO strings including the offset
            const startDateWithOffset = new Date(startDate.getTime() - offset * 60000).toISOString();
            const endDateWithOffset = new Date(endDate.getTime() - offset * 60000).toISOString();

            // Send dates with the offset applied
            PatchMeeting({
                    meetingName: title,
                    description: desc,
                    startOfTheMeeting: startDateWithOffset,
                    endOfTheMeeting: endDateWithOffset,
                    placeName: address.name,
                    lat: address.lat,
                    lon: address.lon,
                    timeZone: timezone,
                }, //@ts-ignore
                link);
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
                variant="unstyled"
                size="md"
                value={title}
                name="title"
                defaultValue={data.name}
                onChange={event => setTitle(event.target.value)}
            />
            <Text size="xl" c="dimmed" fw={500}>Opis</Text>
            <TextInput
                variant="unstyled"
                size="md"
                value={desc}
                name="desc"
                defaultValue={data.desc}
                onChange={event => setDesc(event.target.value)}
            />
            <Group>
                <Text size="xl" c="dimmed" fw={500}>Adres</Text>
                <Tooltip
                    label={"Wpisz adres i kliknij enter. Jeżeli żaden z podpowiadanych nie pasuje, wybierz ostatnią opcję, która będzie wartością jaką wpisałeś."}>
                    <IconHelpCircle size={16} color={theme.colors.gray[5]} />
                </Tooltip>
            </Group>
            <AddressSearchBar
                initialAddress={data.placeName} // Przekaż ostatni adres do AddressSearchBar
                onSelect={(location) => setAddress(location)}
            />
            <Text size="xl" c="dimmed" fw={500}>Data i godzina rozpoczęcia spotkania</Text>
            <DateTimePicker
                valueFormat="DD.MM.YYYY HH:mm"
                defaultValue={startDate}
                placeholder="Wybierz datę"
                locale='pl'
                onChange={(newDate) => setStartDate(newDate)}
                name="startDate"
            />
            <Text size="xl" c="dimmed" fw={500}>Data i godzina zakończenia spotkania</Text>
            <DateTimePicker
                valueFormat="DD.MM.YYYY HH:mm"
                defaultValue={endDate}
                placeholder="Wybierz datę"
                locale='pl'
                name="endDate"
                onChange={(newDate) => setEndDate(newDate)}
            />
            <Text size="xl" c="dimmed" fw={500}>Strefa czasowa spotkania</Text>
            <TimezoneSelectBar onSelect={(selectedTimezone) => setTimezone(selectedTimezone)} />
            <Group style={{
                alignItems: "flex-start", alignContent: "center", justifyContent: "space-between", padding: "10px"
            }}>
                <Group m="lg" ml="0">
                    <Button onClick={handleSubmit}>Zatwierdź zmiany</Button>
                    <Button variant="outline" color="red" onClick={handleRejectChanges}>Odrzuć zmiany</Button>
                </Group>
            </Group>
        </>
    );
};

export default EditMeeting;
