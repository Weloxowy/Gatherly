
import {ExtendedMeeting, Meeting, Person} from "@/lib/interfaces/types";

const date = Date.now();

export const meetings: Meeting[] = [
    { id: 0, date: new Date(date), name: "Ósme urodzinki Sabinki", place: "Młoda 4" },
    { id: 1, date: new Date(date - 4 * 24 * 60 * 60 * 1000), name: "Impreza nad zalewem", place: "Zalew Kielecki" },
    { id: 2, date: new Date(date - 4 * 24 * 60 * 60 * 1000), name: "Koncert", place: "COS Torwar" },
    { id: 3, date: new Date(date + 3 * 24 * 60 * 60 * 1000), name: "Wspólne wyjście", place: "Działka Ilony" },
    { id: 4, date: new Date(date + 7 * 24 * 60 * 60 * 1000), name: "Piknik na plaży", place: "Wejście na plaże nr. 52" },
    { id: 5, date: new Date(date + 12 * 24 * 60 * 60 * 1000), name: "Tworzymy startup!!", place: "online" }
];

export const extendedMeetings: ExtendedMeeting[] = [
    { id: "0", date: new Date(date), dateOfCreation:new Date(date - 76 * 24 * 60 * 60 * 1000),
        name: "Ósme urodzinki Sabinki", placeName: "Młoda 4", desc: "Urodziny niespodzianka!",
        lon:50.88034750100393, lat:20.618371971164006,
        confirmedInvites:[
            {id: '56789', name:"Artur Soboń", avatar:""},
            {id: '14451', name:"Ilona Stopień", avatar:""},
            {id: '23123', name:"Jeremiasz Maron", avatar:""},
            {id: '35351', name:"Anna Wiech", avatar:""}
        ], sendInvites:[
            {id: '12234', name:"Filip Skrońc", avatar:""},
            {id: '13123', name:"Mateusz Stuch", avatar:""},
            {id: '98877', name:"Emilia Wartuń", avatar:""},
            {id: '06543', name:"Romuald Kopytko", avatar:""},
            {id: '24647', name:"Jędrzej Tomidalski", avatar:""}
        ], rejectedInvites: [
            {id: '11457', name:"Renata Gomułka", avatar:""},
            {id: '95556', name:"Ryszald Piłsudski", avatar:""}
        ]
    },
    { id: "1", date: new Date(date - 4 * 24 * 60 * 60 * 1000),
        dateOfCreation:new Date(date - 76 * 24 * 60 * 60 * 1000), name: "Impreza nad zalewem",
        placeName: "Zalew Kielecki", desc: "Urodziny niespodzianka!", lon:50.89054316450333, lat:20.6343669346524,
        confirmedInvites:[
            {id: '56789', name:"Artur Soboń", avatar:""},
            {id: '98877', name:"Emilia Wartuń", avatar:""},
            {id: '06543', name:"Romuald Kopytko", avatar:""}
        ], sendInvites:[
            {id: '12234', name:"Filip Skrońc", avatar:""},
            {id: '14451', name:"Ilona Stopień", avatar:""},
            {id: '35351', name:"Anna Wiech", avatar:""},
            {id: '13123', name:"Mateusz Stuch", avatar:""},
            {id: '24647', name:"Jędrzej Tomidalski", avatar:""}
        ], rejectedInvites: [
            {id: '11457', name:"Renata Gomułka", avatar:""},
            {id: '23123', name:"Jeremiasz Maron", avatar:""},
            {id: '95556', name:"Ryszald Piłsudski", avatar:""}
        ]
    },
    { id: "2", date: new Date(date - 4 * 24 * 60 * 60 * 1000),
        dateOfCreation:new Date(date - 126 * 24 * 60 * 60 * 1000), name: "Koncert",
        placeName: "COS Torwar", desc: "", lon:52.22405765448268, lat:21.04138820807,
        confirmedInvites:[
            {id: '12234', name:"Filip Skrońc", avatar:""},
            {id: '56789', name:"Artur Soboń", avatar:""},
            {id: '98877', name:"Emilia Wartuń", avatar:""},
            {id: '35351', name:"Anna Wiech", avatar:""},
            {id: '13123', name:"Mateusz Stuch", avatar:""},
            {id: '11457', name:"Renata Gomułka", avatar:""},
            {id: '23123', name:"Jeremiasz Maron", avatar:""}
        ], sendInvites:[
            {id: '06543', name:"Romuald Kopytko", avatar:""},
            {id: '14451', name:"Ilona Stopień", avatar:""},
            {id: '24647', name:"Jędrzej Tomidalski", avatar:""}
        ], rejectedInvites: [
            {id: '95556', name:"Ryszald Piłsudski", avatar:""}
        ]
    },
    { id: "3", date: new Date(date + 3 * 24 * 60 * 60 * 1000),
        dateOfCreation:new Date(date - 45 * 24 * 60 * 60 * 1000), name: "Wspólne wyjście",
        placeName: "Działka Ilony", desc: "",
        confirmedInvites:[
            {id: '56789', name:"Artur Soboń", avatar:""},
            {id: '14451', name:"Ilona Stopień", avatar:""},
            {id: '35351', name:"Anna Wiech", avatar:""},
            {id: '13123', name:"Mateusz Stuch", avatar:""},
            {id: '98877', name:"Emilia Wartuń", avatar:""},
            {id: '06543', name:"Romuald Kopytko", avatar:""},
            {id: '24647', name:"Jędrzej Tomidalski", avatar:""},
            {id: '95556', name:"Ryszald Piłsudski", avatar:""}
        ], sendInvites:[

        ], rejectedInvites: [
            {id: '11457', name:"Renata Gomułka", avatar:""},
            {id: '23123', name:"Jeremiasz Maron", avatar:""},
            {id: '12234', name:"Filip Skrońc", avatar:""}
        ]
    },
    { id: "4", date: new Date(date + 7 * 24 * 60 * 60 * 1000),
        dateOfCreation:new Date(date - 46 * 24 * 60 * 60 * 1000), name: "Piknik na plaży",
        placeName: "Wejście na plaże nr. 52", desc: "Urodziny niespodzianka Jeremiasza!", lon:54.41302753883513, lat:18.624968507197625,
        confirmedInvites:[
            {id: '56789', name:"Artur Soboń", avatar:""},
            {id: '06543', name:"Romuald Kopytko", avatar:""},
            {id: '11457', name:"Renata Gomułka", avatar:""},
            {id: '23123', name:"Jeremiasz Maron", avatar:""}
        ], sendInvites:[
            {id: '24647', name:"Jędrzej Tomidalski", avatar:""}
        ], rejectedInvites: [
        ]
    },
    { id: "5", date: new Date(date + 12 * 24 * 60 * 60 * 1000),
        dateOfCreation:new Date(date - 25 * 24 * 60 * 60 * 1000), name: "Tworzymy startup!",
        placeName: "online", desc: "Wejdźcie punktualnie!!",
        confirmedInvites:[
            {id: '56789', name:"Artur Soboń", avatar:""},
            {id: '12234', name:"Filip Skrońc", avatar:""},
            {id: '13123', name:"Mateusz Stuch", avatar:""},
            {id: '24647', name:"Jędrzej Tomidalski", avatar:""},
        ], sendInvites:[
        ], rejectedInvites: [
        ]
    }

];
export function getMeetingById(id: number): Meeting | undefined {
    return meetings.find(meeting => meeting.id === id);
}

export function getExtendedMeetingById(id: string): ExtendedMeeting | undefined {
    return extendedMeetings.find(meeting => meeting.id.match(id));
}

