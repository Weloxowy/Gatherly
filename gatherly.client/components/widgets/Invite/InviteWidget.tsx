import React, { useEffect, useState } from 'react';
import { qrGenerator } from '@/lib/utils/qrGenerator/qrGenerator';
import getPageName from '@/lib/utils/qrGenerator/getPageName';

const InviteWidget: React.FC = () => {
    const [svg, setSvg] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        const fetchQRCode = async () => {
            const link = getPageName(window.location.href)?.toString();
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
        return <div>Loading...</div>; // Możesz zmienić to na dowolny wskaźnik ładowania
    }

    return (
        <>
            {/* Używamy div z dangerouslySetInnerHTML do renderowania SVG */}
            {svg ? (
                <div dangerouslySetInnerHTML={{ __html: svg }} />
            ) : (
                <div>No QR code available</div>
            )}
        </>
    );
};

export default InviteWidget;
