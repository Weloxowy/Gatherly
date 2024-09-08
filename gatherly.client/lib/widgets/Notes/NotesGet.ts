import axiosInstance from '@/lib/utils/AxiosInstance';

async function NotesGet() {
    try {
        const response = await axiosInstance.get('Reminders');
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default NotesGet;

