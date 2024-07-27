import QRCode from 'qrcode';

export async function qrGenerator(url: string) {
    try {
        const qrCodeSvg = await QRCode.toString(url, { type: 'svg' });
        return qrCodeSvg;
        /*
        createWriteStream(outputFilePath).write(qrCodeSvg);

        console.log(`QR kod został zapisany do ${outputFilePath}`);
        */
    } catch (error) {
        console.error('Wystąpił błąd podczas generowania QR kodu:', error);
    }
}
