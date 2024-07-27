import {Button, Flex, Text, TextInput, Title} from "@mantine/core";
import Image from "next/image";
import "./style.css"
import React from "react";

const Welcome: React.FC = () => {

    return (
        <div id={"home"}>
            <div style={{
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center',
                alignItems: 'center', paddingTop: 150,
            }}>
                <div style={{textAlign: "center", marginBottom: '2rem'}} >
                    <Title order={1} style={{fontWeight: 500}}>Planowanie spotkań nigdy
                        nie
                        było takie proste!<br/> Przedstawiamy
                        <b> Gatherly</b>
                    </Title>
                    <div style={{textAlign: "center", marginTop: '1rem'}}>
                        <Text size={"md"}>Twórz spotkania, zapraszaj znajomych, ustalcie wspólnie termin i miejsce
                            spotkania i
                            bawcie się
                            dobrze.</Text>
                    </div>
                </div>
                <Flex direction="row">
                    <TextInput size={"md"} w={'30rem'} variant={"default"} placeholder={"Adres email"}></TextInput>
                    <Button size={"md"}>Rozpocznij</Button>
                </Flex>
                <div style={{padding: '6rem'}}>
                    <Image src={"/screenshotPlaceholder.png"} alt={"Placeholder"} width={900} height={1000}/>
                </div>
            </div>
            <section id={"functions"}>
                <Text>Functions section</Text>
                <div style={{paddingTop: '600px'}}></div>
            </section>
            <section id={"invite"}>
                <Text>Invite section</Text>
            </section>
        </div>
    )
}

export default Welcome;
