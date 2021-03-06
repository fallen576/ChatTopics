$(document).ready(() => {

    connection.start().then(() => connected = true);

    $("#create-topic").submit((e) => {
        e.preventDefault();
        let topic = $("#new-topic").val();

        fetch("/exists?roomName="+topic)
            .then(r => r.json())
            .then(r => {
                if (r.exists) {
                    console.log("room exists");
                    $("#error").text("room exists");
                    return;
                }
                $("#error").text("");
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
    });    

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

    $(window).bind("beforeunload", function (e) {
        if (currentRoom != "") {
            leave(currentRoom);
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
    if (!connected) {
        connection.start()
            .then(() => connection.invoke('JoinRoom', room))
            .then((history) => {
                for (var i in history) {
                    let obj = history[i];
                    insertNewMessage(obj.userName, obj.message, obj.timeStamp);
                }
                connection.on("recieveMessage", (userMessage) => {
                    console.log(userMessage);
                    insertNewMessage(userMessage.userName, userMessage.message, userMessage.timeStamp);
                })
                // connection.on('send_message', logMessage) // needed for working example
            })
    }
    else {
        connection.invoke('JoinRoom', room)
            .then((history) => {
            for (var i in history) {
                let obj = history[i];
                insertNewMessage(obj.userName, obj.message, obj.timeStamp);
            }
            connection.on("recieveMessage", (userMessage) => {
                console.log(userMessage);
                insertNewMessage(userMessage.userName, userMessage.message, userMessage.timeStamp);
            })
            // connection.on('send_message', logMessage) // needed for working example
        })
    }
    /*
    connection.start()
    .then(() => connection.invoke('JoinRoom', room ))
        .then((history) => {
            for (var i in history) {
                let obj = history[i];
                insertNewMessage(obj.userName, obj.message, obj.timeStamp);
            }
            connection.on("recieveMessage", (userMessage) => {
                console.log(userMessage);
                insertNewMessage(userMessage.userName, userMessage.message, userMessage.timeStamp);
            })
        // connection.on('send_message', logMessage) // needed for working example
    })
    */
}

const leave = () => connection.invoke('LeaveRoom', currentRoom )
    .then(() => {
        currentRoom = '';
        connected = false;
        // function reference needs to be the same to work
        // connection.off('send_message', m => console.log(m)) // doesn't work
        // connection.off('send_message', logMessage) // works  
        connection.off('recieveMessage')
        return connection.stop()
    })

const joinTopic = (id) => {
    let topic = $("#" + id).text();
    $("#join_" + id).prop('disabled', true);
    $("#leave_" + id).prop('disabled', false);

    console.log("joining " + id);

    $('#topic-list td button').each(function (i) {        
        $(this).prop('disabled', true);
    });

    $('#topic-list tbody tr:nth-child(' + (Number.parseInt(id.split("_")[1]) + 1) + ') td:nth-child(2) button').prop("disabled", true);
    $('#topic-list tbody tr:nth-child(' + (Number.parseInt(id.split("_")[1]) + 1) + ') td:nth-child(3) button').prop("disabled", false);

    join(topic);

    //get who is in the topic
    fetch("users/" + topic)
        .then(r => r.json())
        .then(resp => {
            for (var i in resp) {
                let user = resp[i];
                insertNewUser(user.userName);
            }
        })
        .catch();
}

const leaveTopic = (id) => {
    let topic = $("#" + id).text();
    //$("#join_" + id).prop('disabled', false);
    //$("#leave_" + id).prop('disabled', true);

    $('#topic-list td button').each(function (i) {
        if ($(this).attr("id").indexOf("leave") > -1) {
            $(this).prop('disabled', true);
        }
        else {
            $(this).prop('disabled', false);
        }
    });

    leave(topic);
    //clear chats
    $('#chat-messages').find("tr:not(:nth-child(1))").remove();
    //clear user
    removeUser();
    //clear user's in topic list since they will no longer be in a topic
    $("#user-list tbody tr").slice(1).remove()
}

async function login() {
    let res = await fetch(`/spalogin?username=${$("#username").val()}`);

    if (res.ok) {
        location.href = "/";
    }
    let body = await res.json();
    $("#error").text(body.message)
}

connection.on("JoinTopic", (user) => {
    insertNewUser(user);
});

connection.on("LeaveTopic", (user) => {
    removeUser(user);
});

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

const insertNewUser = (user) => {
    let row = '<tr id="' + user + '" class="topic-entry"><td class="topics">' + user + '</td></tr>';
    $("#user-list tr:last").after(row);
}

const removeUser = (usr) => {
    $("#" + usr).remove();
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
        });
}