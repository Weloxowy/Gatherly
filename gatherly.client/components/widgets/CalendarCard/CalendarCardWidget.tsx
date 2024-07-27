import React from "react";
import classes from "./CalendarCardWidget.module.css";

const CalendarCardWidget : React.FC = () => {
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
                Masz dziś 0 spotkań.
            </div>
        </>
    )
}

export default CalendarCardWidget;
