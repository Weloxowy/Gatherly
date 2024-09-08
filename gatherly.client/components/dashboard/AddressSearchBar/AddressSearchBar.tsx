import React, { useState, useRef } from 'react';
import { Combobox, Loader, TextInput, useCombobox } from '@mantine/core';
import GetMeeting from "@/lib/widgets/GetMeeting";

const AddressSearchBar: React.FC<{ initialAddress?: string, onSelect?: (location: any) => void }> = ({ initialAddress, onSelect }) => {
    const combobox = useCombobox({
        onDropdownClose: () => combobox.resetSelectedOption(),
    });

    const [value, setValue] = useState(initialAddress);
    const [data, setData] = useState<any[] | null>(null);
    const [loading, setLoading] = useState(false);
    const [empty, setEmpty] = useState(false);
    const abortController = useRef<AbortController>();

    const fetchOptions = (query: string) => {
        abortController.current?.abort();
        abortController.current = new AbortController();
        setLoading(true);
        fetch(`/api/getPlaceInfo?query=${encodeURIComponent(query)}`, {
            method: 'GET',
            signal: abortController.current.signal
        })
            .then((response) => response.json())
            .then((uniqueResults) => {
                if (uniqueResults.length >= 0) {
                    uniqueResults.push({
                        value: value,
                        key: 'value_field',
                        lat: 0,
                        lon: 0,
                    });
                }

                setData(uniqueResults);
                setEmpty(uniqueResults.length === 0);
                setLoading(false);
            })
            .catch(() => {
                setLoading(false);
            });
    };

    const handleSearch = () => {
        if (value && value.length >= 3) {
            fetchOptions(value);
        } else {
            setData(null);
            setEmpty(false);
        }
    };

    const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
        if (event.key === 'Enter') {
            handleSearch();
            combobox.openDropdown();
        }
    };

    const options = (data || []).map((item) => (
        item.key === 'value_field' ? (
            <Combobox.Option value={item.value} key={item.key} style={{ backgroundColor: '#7048e8', color: 'white' }}>
                {item.value}
            </Combobox.Option>
        ) : (
            <Combobox.Option value={item.value} key={item.key}>
                {item.value}
            </Combobox.Option>
        )
    ));

    return (
        <Combobox
            transitionProps={{ transition: 'fade' }}
            position={'top'}
            onOptionSubmit={(optionValue) => {
                const selectedLocation = data?.find((item) => item.value === optionValue);
                if (selectedLocation && onSelect) {
                    onSelect({
                        lat: selectedLocation.lat,
                        lon: selectedLocation.lon,
                        name: optionValue,
                    });
                }
                setValue(optionValue);
                combobox.closeDropdown();
            }}
            withinPortal={false}
            store={combobox}
        >
            <Combobox.Target>
                <TextInput
                    placeholder={initialAddress || 'Podaj adres'}
                    value={value}
                    onChange={(event) => {
                        setValue(event.currentTarget.value);
                        if (event.currentTarget.value.length >= 3) {
                            combobox.openDropdown();
                        } else {
                            setData(null);
                            combobox.closeDropdown();
                        }
                    }}
                    onClick={() => combobox.openDropdown()}
                    onFocus={() => {
                        if (value && value.length >= 3) {
                            fetchOptions(value);
                            combobox.openDropdown();
                        }
                    }}
                    onBlur={() => combobox.closeDropdown()}
                    onKeyDown={handleKeyDown}
                    rightSection={loading && <Loader size={18} />}
                />
            </Combobox.Target>

            <Combobox.Dropdown hidden={data === null}>
                <Combobox.Options mah={200} style={{ overflowY: 'auto' }}>
                    {options}
                    {empty && <Combobox.Empty>Nie znaleziono wyników</Combobox.Empty>}
                </Combobox.Options>
            </Combobox.Dropdown>
        </Combobox>
    );
};

export default AddressSearchBar;
