
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
    meetingId: string;
    date: Date;
    name: string;
    place: string;
}

export interface Person {
    personId: string;
    name: string;
    avatar: string;
}

export interface AuthReturn{
    jwt : string,
    refresh : string
}

export interface Meeting {
    meetingId: string;
    date: Date;
    name: string;
    place: string;
}

export interface ExtendedMeeting {
    id: string;
    ownerId: string,
    ownerName: string,
    startOfTheMeeting: Date;
    endOfTheMeeting: Date;
    creationTime: Date,
    isMeetingTimePlanned : boolean;
    desc: string;
    name: string;
    placeName: string;
    lon?: number;
    lat?: number;
    confirmedInvites : Person[];
    sendInvites : Person[];
    rejectedInvites : Person[];
    isRequestingUserAnOwner : boolean;
}

export interface AddressInfo {
    lon? : number,
    lat?: number,
    name : string,
}

export interface Invitation{
    id: string,
    userId : string,
    meetingId : string,
    validTime: Date
}

export interface InvitationMeeting{
    InvitationId: string,
    userId : string,
    meetingId : string,
    validTime: Date
    OwnerId: string,
    MeetingName: string,
    Description: string,
    PlaceName: string,
    StartOfTheMeeting: Date,
    EndOfTheMeeting: Date,
    TimeZone: string;
}

export interface UserInfo{
    name : string,
    email : string,
    avatarName : string
}
