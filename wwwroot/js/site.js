var currentRoom = ""

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .build();

const send = (message) => connection.invoke('SendMessage', message, currentRoom )

const create = (room) => fetch('/create?roomName=' + room)

const list = () => fetch('/list').then(r => r.json()).then(r => console.log("rooms", r))

// const logMessage = (m) => console.log(m) // needed for working example

const join = (room) => connection.start()
    .then(() => connection.invoke('JoinRoom', room ))
    .then((history) => {
        console.log('message history', history)
        currentRoom = room
        connection.on('recieveMessage', m => console.log(m))
        // connection.on('send_message', logMessage) // needed for working example
    })

const leave = () => connection.invoke('LeaveRoom', currentRoom )
    .then(() => {
        currentRoom = ''
        // function reference needs to be the same to work
        // connection.off('send_message', m => console.log(m)) // doesn't work
        // connection.off('send_message', logMessage) // works
        connection.off('recieveMessage')
        return connection.stop()
    })
