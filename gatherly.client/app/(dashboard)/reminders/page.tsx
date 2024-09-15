"use client"
import NotesWidget from "@/components/widgets/Notes/NotesWidget";
import classes from "./Reminders.module.css";
import {Title} from "@mantine/core";
import React from "react";

export default function Reminders() {
    return (<div className={classes.main}>
            <div className={classes.name}>
                <Title order={2}>Przypomnienia</Title>
                Zapisz krótkie notatki
            </div>
        <div style={{
            position: 'relative',
            paddingTop: '1rem',
            paddingLeft: '1rem',
            width: '70vw',
            height: 'calc(100vh - 10rem)',
            zIndex: 10,
            display: 'flex',
            flexDirection: 'column'
        }}>
            <NotesWidget/>
        </div>
    </div>);
}
