import React, { useState, useRef } from 'react';
import { Combobox, InputBase, Loader, Input, useCombobox } from '@mantine/core';
import axiosInstance from '@/lib/utils/AxiosInstance';

const TimezoneSelectBar: React.FC<{ onSelect?: (timezone: any) => void }> = ({ onSelect }) => {
    const combobox = useCombobox({
        onDropdownClose: () => combobox.resetSelectedOption(),
    });

    const [value, setValue] = useState('');
    const [data, setData] = useState<any[] | null>(null);
    const [loading, setLoading] = useState(false);
    const [empty, setEmpty] = useState(false);
    const abortController = useRef<AbortController>();

    const fetchOptions = () => {
        abortController.current?.abort();
        abortController.current = new AbortController();
        setLoading(true);
        axiosInstance
            .get(`Meetings/getAllTimeZones`)
            .then((response) => {
                const res = response.data;
                const results = Object.entries(res).map(([key, value]) => ({
                    value: key,
                    key: value
                }));
                setData(results);
                setEmpty(results.length === 0);
                setLoading(false);
                combobox.toggleDropdown();
            })
            .catch(() => {
                setLoading(false);
            });
    };

    const options = (data || []).map((item) => (
        <Combobox.Option value={item.value} key={item.key}>
            {item.key}
        </Combobox.Option>
    ));

    return (
        <Combobox
            store={combobox}
            withinPortal={false}
            onOptionSubmit={(val) => {
                const foundItem = data?.find((item) => item.value === val);
                if (foundItem) {
                    setValue(foundItem.key);
                    // Wywołaj funkcję onSelect z wybraną strefą czasową, jeśli jest dostępna
                    if (onSelect) {
                        onSelect(foundItem.value); // lub foundItem.key, w zależności od tego, co chcesz przekazać
                    }
                }
                combobox.closeDropdown();
            }}
        >
            <Combobox.Target>
                <InputBase
                    component="button"
                    type="button"
                    pointer
                    rightSection={loading ? <Loader size={18} /> : <Combobox.Chevron />}
                    onClick={() => fetchOptions()}
                    rightSectionPointerEvents="none"
                >
                    {value || <Input.Placeholder>Wybierz strefę czasową</Input.Placeholder>}
                </InputBase>
            </Combobox.Target>

            <Combobox.Dropdown>
                <Combobox.Options mah={200} style={{ overflowY: 'auto' }}>
                    {loading ? <Combobox.Empty>Loading...</Combobox.Empty> : options}
                    {empty && <Combobox.Empty>Nie znaleziono wyników</Combobox.Empty>}
                </Combobox.Options>
            </Combobox.Dropdown>
        </Combobox>
    );
};

export default TimezoneSelectBar;
