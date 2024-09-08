'use client'

import React, {useEffect, useState} from "react";
import classes from "./CalendarCardWidget.module.css";
import CalendarCardGet from "@/lib/widgets/CalendarCard/CalendarCardGet";

const CalendarCardWidget : React.FC = () => {
    const [numberOfMeetings, setNumberOfMeetings] = useState(0);

    useEffect(() => {
        (async () => {
            const meetingsCount = await CalendarCardGet();
            setNumberOfMeetings(meetingsCount);
        })();
    }, []);

    const currentDate = new Date();
    const daysOfWeek = [
        'Niedziela',
        'Poniedziałek',
        'Wtorek',
        'Środa',
        'Czwartek',
        'Piątek',
        'Sobota'
    ];
    const months = [
        'Styczeń',
        'Luty',
        'Marzec',
        'Kwiecień',
        'Maj',
        'Czerwiec',
        'Lipiec',
        'Sierpień',
        'Wrzesień',
        'Październik',
        'Listopad',
        'Grudzień'
    ];

    const day = daysOfWeek[currentDate.getDay()];
    const date = currentDate.getDate();
    const month = months[currentDate.getMonth()];

    return (
        <>
            <div className={classes.day}>
                {day}
            </div>
            <div className={classes.number}>
                {date}
            </div>
            <span>
        {month}
      </span>
            <div>
                {
                    numberOfMeetings == 0 ? "Nie masz dziś spotkań" : (numberOfMeetings == 1 ? "Masz dziś 1 spotkanie": "Masz dziś "+numberOfMeetings+" spotkań")
                }
            </div>
        </>
    )
}

export default CalendarCardWidget;
