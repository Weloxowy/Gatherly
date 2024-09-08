import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

connection.on("ReceiveMessage", (message) => {
    console.log("Odebrana wiadomość:", message);
});

connection.start()
    .then(() => console.log("Połączono z Hubem"))
    .catch(err => console.error(err.toString()));

function sendMessage(content: string, meetingId : string) {
    connection.invoke("SendMessage", meetingId, content)
        .catch(err => console.error(err.toString()));
}
