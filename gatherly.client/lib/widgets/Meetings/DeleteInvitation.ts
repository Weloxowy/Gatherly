import axiosInstance from '@/lib/utils/AxiosInstance';

async function DeleteInvitation(invitationId : string) {

    try {
        const response = await axiosInstance.delete('Invitations/delete/'+invitationId);
        return response.data;
    } catch (error) {
        throw error;
    }
}

export default DeleteInvitation;

