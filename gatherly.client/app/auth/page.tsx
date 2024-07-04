import {
    TextInput,
    Checkbox,
    Anchor,
    Paper,
    Title,
    Text,
    Container,
    Group,
    Button,
} from '@mantine/core';
import getData from "@/lib/getData";

export default async function Auth() {
    const data = getData();
    return (
        <main>
            <Container size={420} my={40}>
                <Title order={2} ta="center">
                    Witamy!
                </Title>
                <Text c="dimmed" size="md" ta="center" mt={5}>
                    Wpisz poniżej adres mailowy aby się zalogować
                </Text>
                <Paper withBorder shadow="md" p={30} mt={30} radius="md">
                    <TextInput size="md" label="Email" placeholder="mail@gatherly.com" required/>
                    <Group justify="space-between" mt="lg">
                        <Checkbox size="md" label="Pamiętaj mnie"/>
                        <Anchor component="button" size="md">
                            Odzyskiwanie dostępu
                        </Anchor>
                    </Group>
                    <Button fullWidth mt="lg">
                        Dalej
                    </Button>
                    Encje użytkowników:
                    {data}
                </Paper>
            </Container>
        </main>
    );
}
