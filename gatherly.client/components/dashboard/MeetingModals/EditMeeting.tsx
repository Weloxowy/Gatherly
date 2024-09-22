'use client';
import React, { useState } from "react";
import { Button, Group, Text, TextInput, Tooltip, useMantineTheme } from "@mantine/core";
import {ExtendedMeeting, MeetingErrors} from "@/lib/interfaces/types";
import AddressSearchBar from "@/components/dashboard/AddressSearchBar/AddressSearchBar";
import { DateTimePicker } from "@mantine/dates";
import 'dayjs/locale/pl';
import {closeAllModals, modals} from "@mantine/modals";
import TimezoneSelectBar from "@/components/dashboard/TimezoneSelectBar/TimezoneSelectBar";
import { IconHelpCircle } from "@tabler/icons-react";
import PatchMeeting from "@/lib/widgets/Meetings/PatchMeeting";
import getPageName from "@/lib/utils/qrGenerator/getPageName";
import adjustTimeToLocal from "@/lib/widgets/Meetings/adjustTimeToLocal";
import dayjs from "dayjs";
import {addNotification} from "@/lib/utils/notificationsManager";

// Helper function to calculate the offset in minutes
const getTimezoneOffset = (date: Date) => {
    return date.getTimezoneOffset(); // Returns the difference in minutes from UTC
}

interface MeetingDetailsProps {
    data: ExtendedMeeting;
}

const EditMeeting: React.FC<MeetingDetailsProps> = ({ data }) => {
    const [loading, setLoading] = useState(false);
    const [title, setTitle] = useState(data.name);
    const [desc, setDesc] = useState(data.desc);
    const [startDate, setStartDate] = useState<Date | null>(dayjs(adjustTimeToLocal(data.startOfTheMeeting,data.timezoneOffset)).toDate());
    const [endDate, setEndDate] = useState<Date | null>(dayjs(adjustTimeToLocal(data.endOfTheMeeting,data.timezoneOffset)).toDate());
    const [address, setAddress] = useState({ lat: data.lat, lon: data.lon, name: data.placeName });
    const [timezone, setTimezone] = useState<string | undefined>();
    const link = getPageName(window.location.href)?.toString();
    const [errors, setErrors] = useState<MeetingErrors>({});
    const handleSubmit = async () => {
        setLoading(true);
        const validationErrors: MeetingErrors = {};
        if (!title) validationErrors.title = "Nazwa spotkania jest wymagana";
        if (!desc) validationErrors.desc = "Opis spotkania jest wymagany";
        if (!startDate) validationErrors.startDate = "Data rozpoczęcia spotkania jest wymagana";
        if (!endDate) validationErrors.endDate = "Data zakończenia spotkania jest wymagana";
        if (!address.name) validationErrors.address = "Adres spotkania jest wymagany";
        if (startDate && endDate && dayjs(startDate).unix() > dayjs(endDate).unix())
            validationErrors.startDate = "Data rozpoczęcia nie może być później niż data zakończenia";

        if (Object.keys(validationErrors).length > 0) {
            setErrors(validationErrors);
            setLoading(false);
            return;
        }

        try {
            if (startDate && endDate) {
                const offset = getTimezoneOffset(startDate);
                const startDateWithOffset = new Date(startDate.getTime() - offset * 60000).toISOString();
                const endDateWithOffset = new Date(endDate.getTime() - offset * 60000).toISOString();

                await PatchMeeting({
                    meetingName: title,
                    description: desc,
                    startOfTheMeeting: startDateWithOffset,
                    endOfTheMeeting: endDateWithOffset,
                    placeName: address.name,
                    lat: address.lat,
                    lon: address.lon,
                    timeZone: timezone,
                    // @ts-ignore
                }, link);
            }
            addNotification({
                title: 'Sukces',
                message: 'Zmiany zostały zapisane pomyślnie.',
                color: 'green',
            });
        } catch (error) {
            addNotification({
                title: 'Wystąpił błąd',
                message: 'Modyfikacja spotkania zakończyła się niepowodzeniem',
                color: 'red',
            });
            //console.error("Error while patching meeting:", error);
        } finally {
            setLoading(false);
            closeAllModals();
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
                placeholder={"Podaj nazwę spotkania"}
                defaultValue={data.name}
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
                placeholder={"Podaj opis spotkania"}
                defaultValue={data.desc}
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
                    label={"Wpisz adres i kliknij enter. Jeżeli żaden z podpowiadanych nie pasuje, wybierz ostatnią opcję, która będzie wartością jaką wpisałeś."}>
                    <IconHelpCircle size={16} color={theme.colors.gray[5]} />
                </Tooltip>
            </Group>
            <AddressSearchBar
                initialAddress={data.placeName}
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
                defaultValue={startDate}
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
                defaultValue={endDate}
                placeholder="Wybierz datę"
                locale='pl'
                name="endDate"
                error={errors.endDate}
                onChange={newDate => {setEndDate(newDate)
                    setErrors(prevErrors => ({
                        ...prevErrors,
                        endDate: undefined
                    }));
                }}
            />

            <Text size="xl" c="dimmed" fw={500}>Strefa czasowa spotkania</Text>
            <TimezoneSelectBar  onSelect={(timezone) => {
                setTimezone(timezone);
                setErrors(prevErrors => ({
                    ...prevErrors,
                    timezone: undefined
                }));
            }}/>
            <Group style={{
                alignItems: "flex-start", alignContent: "center", justifyContent: "space-between", padding: "10px"
            }}>
                <Group m="lg" ml="0">
                    <Button loading={loading} onClick={handleSubmit}>Zatwierdź zmiany</Button>
                    <Button loading={loading} variant="outline" color="red" onClick={handleRejectChanges}>Odrzuć zmiany</Button>
                </Group>
            </Group>
        </>
    );
};

export default EditMeeting;
