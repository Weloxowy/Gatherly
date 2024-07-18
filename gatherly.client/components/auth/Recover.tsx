﻿import React from "react";
import {Anchor, Button, Group, TextInput} from "@mantine/core";

const Recover : React.FC = ({setAuthMethod, options}) => {

    return(
        <>
            <TextInput size="md" label="Email" placeholder="mail@gatherly.com" required/>
            <Group justify="space-between" mt="lg">
                <Anchor component="button" size="md" onClick={() => setAuthMethod(options.loginTraditional)}>
                    Logowanie
                </Anchor>
                <Anchor component="button" size="md" onClick={() => setAuthMethod(options.register)}>
                    Rejestracja
                </Anchor>
            </Group>
            <Button fullWidth mt="lg">
                Resetuj hasło
            </Button>
        </>
    )
}

export default Recover;
