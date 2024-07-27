export default function getPageName(url: string): string | undefined {
    try {
        let processedUrl = url;
        if (processedUrl.includes('?')) {
            processedUrl = processedUrl.split('?')[0];
        }
        const pageName = processedUrl.split('/').pop();
        return pageName;
    } catch (error) {
        console.error('Błąd podczas przetwarzania URL:', error);
        return undefined;
    }
}
