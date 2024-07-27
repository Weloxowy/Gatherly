"use client"
import NavPanel from "@/components/dashboard/navPanel/navPanel";
import {Burger, Flex} from "@mantine/core";
import React from "react";
import {useDisclosure} from "@mantine/hooks";

export default function DashboardLayout({
                                            children,
                                        }: {
    children: React.ReactNode
}) {
    const [opened, {toggle}] = useDisclosure();
    return (<section>
        <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm"/>
        <Flex
            gap="sm"
            justify="flex-start"
            align="flex-start"
            direction="row"
            wrap="wrap"
        >
            <NavPanel/>
            {children}
        </Flex>
    </section>)
}
