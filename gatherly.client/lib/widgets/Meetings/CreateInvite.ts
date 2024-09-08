import axiosInstance from '@/lib/utils/AxiosInstance';

async function CreateInvite(userEmail : string, meetingId : string) {

    try {
        const response = await axiosInstance.post('Invitations/create',JSON.stringify({userEmail: userEmail, meetingId: meetingId}));
        return response;
    } catch (error) {
        throw error;
    }
}

export default CreateInvite;

