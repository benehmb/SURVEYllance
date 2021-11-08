let joinId;

// <editor-fold desc="Websocket-Connection">
// Connect to Websocket
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/creator")
    .configureLogging(signalR.LogLevel.Information)
    .build();

//TODO: At the moment we can only create a room, not join one.
async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
        joinId = await connection.invoke("CreateRoom");
        console.log("New SURVEYllance-Session with JoinId: " + joinId);
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(async () => {
    await start();
});

// Start the connection.
start();

//TODO: Do we want a retry? this wil create a new room, so we need reusable sessions or no retry

// Retry if connection loses
connection.onclose(async () => {
    await start();
});
// </editor-fold>

//<editor-fold desc="API-Methods"
connection.on("OnNewSurveyResult", (survey, answer) => {
    //Find Survey, maybe give each survey unique id?
    //Update Answer
});


connection.on("OnNewQuestion", (question) => {
    let questionDOM = new QuestionDOM(question);
    document.getElementById('questions').appendChild(questionDOM.domObject);
});
//</editor-fold>
