import React, { useEffect, useState } from 'react';
import { qrGenerator } from '@/lib/utils/qrGenerator/qrGenerator';
import {IconHelpCircle} from "@tabler/icons-react";
import {Tooltip} from "@mantine/core";

const InviteIcon: React.FC = () => {
    const [svg, setSvg] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        const fetchQRCode = async () => {
            const link = window.location.href.toString();
            if (link) {
                try {
                    const qrSvg = await qrGenerator(link);
                    setSvg(qrSvg || null);
                } catch (error) {
                    console.error('Błąd podczas generowania QR kodu:', error);
                    setSvg(null);
                }
            } else {
                setSvg(null);
            }
            setLoading(false);
        };

        fetchQRCode();
    }, []);

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <>
            <Tooltip label={"Zeskanuj kod QR do szybkiego przejścia do spotkania. Kliknij aby przejść do zapraszania osób do spotkania."}>
                <IconHelpCircle />
            </Tooltip>
            {svg ? (
                <div dangerouslySetInnerHTML={{ __html: svg }} />
            ) : (
                <div>No QR code available</div>
            )}
        </>
    );
};

export default InviteIcon;
