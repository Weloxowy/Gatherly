
//Authentication types of components
export const authOptions = {
    loginTraditional: 'loginTraditional',
    loginByCode: 'loginByCode',
    register: 'register',
    recover: 'recover'
};

export type AuthOptions = typeof authOptions;

export interface AuthProps {
    setAuthMethod: (method: string) => void;
    options: AuthOptions;
}

//Meetings types
export interface Meeting {
    id: number;
    date: Date;
    name: string;
    place: string;
}

export interface Person {
    id: string;
    name: string;
    avatar: string;
}

export interface AuthReturn{
    jwt : string,
    refresh : string
}

export interface Meeting {
    id: number;
    date: Date;
    name: string;
    place: string;
}

export interface ExtendedMeeting {
    id: string;
    date: Date;
    dateOfCreation: Date;
    desc: string;
    name: string;
    placeName: string;
    lon?: number;
    lat?: number;
    confirmedInvites : Person[];
    sendInvites : Person[];
    rejectedInvites : Person[];
}
