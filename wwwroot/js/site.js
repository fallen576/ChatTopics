$(document).ready(() => {
    $("#create-topic").submit((e) => {
        e.preventDefault();
        let topic = $("#new-topic").val();
        if (!connected) {
            console.log("call connect async");
            connection.start()
                .then(() => {
                    connected = true;
                    create(topic);
                    connection.invoke('NotifyTopicCreate', topic)
                        .then(() => {
                            connection.stop();
                            connected = false;
                        })
                });
        }
        else {
            create(topic);
            connection.invoke('NotifyTopicCreate', topic)
        }
    });

    //thinking make the url like, /?room=ben. then onload i know what room theyre in and can pre populate the chat.

    $("#login").submit((e) => {
        e.preventDefault();
        login();
    });

    $("#chat-form").submit((e) => {
        e.preventDefault();
        let msg = $("#new-message").val();
        console.log("current room " + currentRoom);
        if (currentRoom != "" && msg != "") {
            send(msg);
            $("#new-message").val('');
        }
    });

})
var currentRoom = ""
var connected = false;

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .build();

const send = (message) => connection.invoke('SendMessage', message, currentRoom )

const create = (room) => fetch('/create?roomName=' + room)
    .then(response => {
        console.log(response.redirected);
        if (!response.redirected) {
            $("#new-topic").val("");
        }
    })
    .catch();

const list = () => fetch('/list').then(r => r.json()).then(r => console.log("rooms", r))

// const logMessage = (m) => console.log(m) // needed for working example

const join = (room) => {
    currentRoom = room;

    connection.start()
    .then(() => connection.invoke('JoinRoom', room ))
        .then((history) => {
            connection.on("recieveMessage", (userMessage) => {
                console.log(userMessage);
                insertNewMessage(userMessage.userName, userMessage.message, userMessage.timeStamp);
            })
        // connection.on('send_message', logMessage) // needed for working example
    })
}

const leave = () => connection.invoke('LeaveRoom', currentRoom )
    .then(() => {
        currentRoom = ''
        // function reference needs to be the same to work
        // connection.off('send_message', m => console.log(m)) // doesn't work
        // connection.off('send_message', logMessage) // works  
        connection.off('recieveMessage')
        return connection.stop()
    })

const joinTopic = (id) => {
    let topic = $("#" + id).text();

    console.log("joining topic of " + topic + " id= " + id);

    $("#join_" + id).prop('disabled', true);
    $("#leave_" + id).prop('disabled', false);
    join(topic);
}

const leaveTopic = (id) => {
    let topic = $("#" + id).text();
    $("#join_" + id).prop('disabled', false);
    $("#leave_" + id).prop('disabled', true);
    leave(topic);

}

async function login() {
    let res = await fetch(`/spalogin?username=${$("#username").val()}`);

    if (res.ok) {
        location.href = "/";
    }
    let body = await res.json();
    $("#error").text(body.message)
}

connection.on("TopicCreate", (topic) => {
    var first = true;
    var i = 1;
    $(".topic-entry").each((j, tr) => {
        i += j;
        first = false;
        console.log(i + ' ' + tr);
    });
    let row = '<tr class="topic-entry"><td class="topics"><span id="topic_'+i+'" style="float:left">' + topic + '</span></td><td><button id="join_topic_' + i + '" class="btn btn-primary" onclick="joinTopic(\'topic_' + i + '\')">Join Topic</button></td><td><button disabled id="leave_topic_' + i + '" class="btn btn-danger" onclick="leaveTopic(\'topic_' + i + '\')">Leave Topic</button></td></tr>';

    if (first) {
        i = 0;
        let row = '<tr class="topic-entry"><td class="topics"><span id="topic_'+i+'" style="float:left">' + topic + '</span></td><td><button id="join_topic_' + i + '" class="btn btn-primary" onclick="joinTopic(\'topic_' + i + '\')">Join Topic</button></td><td><button disabled id="leave_topic_' + i + '" class="btn btn-danger" onclick="leaveTopic(\'topic_' + i + '\')">Leave Topic</button></td></tr>';
        $('#topic-list tr:last').after(row);
    }
    else {
        $('#topic-list tr:last').after(row);
    }
})

const insertNewMessage = (usr, msg, time) => {    
    let row = '<tr class="chats"><td><div class="left"><strong>' + usr + '</strong></div><span>' + msg + '</span><span class="time-right" id="message-time">' + time + '</span></td></tr>';
    $('#chat-messages tr:last').after(row);
}

const connectPLZZZ = () => {

    //how can I wait for this?
    connection.start()
        .then(() => {
            console.log("connected!");
        });
}

async function connectToHub() {    
    try {
        await connection.start();
        console.log("SignalR Connected.");
        connected = true;
    } catch (err) {
        setTimeout(connectToHub, 1000);
    }
}

async function disconnectFromHub() {
    return await connection.stop()
        .then(() => {
            connected = false;
            console.log("okay to move on to after connect");
        });
}