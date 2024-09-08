import axiosInstance from '@/lib/utils/AxiosInstance';

async function NotesPut(data : string) {
    try {
        const response = await axiosInstance.put('Reminders',data);
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default NotesPut;
