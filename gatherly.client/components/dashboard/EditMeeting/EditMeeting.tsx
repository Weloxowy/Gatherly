'use client'
import React, {useState} from "react";
import {Button, Group, Text, TextInput, Tooltip, useMantineTheme} from "@mantine/core";
import {ExtendedMeeting} from "@/lib/interfaces/types";
import AddressSearchBar from "@/components/dashboard/AddressSearchBar/AddressSearchBar";
import {DateTimePicker} from "@mantine/dates";
import 'dayjs/locale/pl';
import {closeAllModals} from "@mantine/modals";
import TimezoneSelectBar from "@/components/dashboard/TimezoneSelectBar/TimezoneSelectBar";
import {IconHelpCircle} from "@tabler/icons-react";
import PatchMeeting from "@/lib/widgets/Meetings/PatchMeeting";
import getPageName from "@/lib/utils/qrGenerator/getPageName";

interface MeetingDetailsProps {
    data: ExtendedMeeting;
}

const EditMeeting: React.FC<MeetingDetailsProps> = ({data}) => {
    const [title, setTitle] = useState(data.name);
    const [desc, setDesc] = useState(data.desc);
    const [startDate, setStartDate] = useState<Date | null>(data.startOfTheMeeting);
    const [endDate, setEndDate] = useState<Date | null>(data.endOfTheMeeting);
    const [address, setAddress] = useState({lat: data.lat, lon: data.lon, name: data.placeName});
    const [timezone, setTimezone] = useState();
    const link = getPageName(window.location.href)?.toString();
    const handleSubmit = () => {
        PatchMeeting({
                meetingName: title,
                description: desc,
                startOfTheMeeting: startDate,
                endOfTheMeeting: endDate,
                placeName: address.name,
                lat: address.lat,
                lon: address.lon,
                timeZone: timezone,
            },//@ts-ignore
            link)
    };

    const handleRejectChanges = () => {
        closeAllModals();
    };
    const theme = useMantineTheme();
    return (<>
            <Text size="xl" c="dimmed" fw={500}>Nazwa spotkania</Text>
            <TextInput
                variant="unstyled"
                size="md"
                value={title}
                name="title" defaultValue={data.name}
                onChange={event => setTitle(event.target.value)}
            />
            <Text size="xl" c="dimmed" fw={500}>Opis</Text>
            <TextInput
                variant="unstyled"
                size="md"
                value={desc}
                name="desc" defaultValue={data.desc}
                onChange={event => setDesc(event.target.value)}
            />
            <Group>
                <Text size="xl" c="dimmed" fw={500}>Adres</Text>
                <Tooltip
                    label={"Wpisz adres i kliknij enter. Jeżeli żaden z podpowiadanych nie pasuje, wybierz ostatnią opcję, która będzie wartością jaką wpisałeś."}>
                    <IconHelpCircle size={16} color={theme.colors.gray[5]}/>
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
                onChange={(newDate) => setEndDate(newDate)}
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
            <TimezoneSelectBar onSelect={(selectedTimezone) => setTimezone(selectedTimezone)}/>
            <Group style={{
                alignItems: "flex-start", alignContent: "center", justifyContent: "space-between", padding: "10px"
            }}>
                <Button onClick={handleSubmit}>Zatwierdź zmiany</Button>
                <Button variant="outline" color="red" onClick={handleRejectChanges}>Odrzuć zmiany</Button>
            </Group>
        </>);
};

export default EditMeeting;
