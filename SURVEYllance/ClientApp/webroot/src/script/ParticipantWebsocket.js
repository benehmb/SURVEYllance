// <editor-fold desc="Websocket-Connection">
// Connect to Websocket
let joinId

if (typeof(Storage) !== "undefined") {
    //Get value from localStorage (See readme.md for more details what to save in the localStorage)
    let type = sessionStorage.getItem("type");
     joinId = sessionStorage.getItem("joinId");

    //Check if type and joinId are set
    if ((type === null || type === undefined) || (joinId === null || joinId === undefined)) {
        M.toast({html: 'You need to join a room first!'});
        throw new Error("No room or joinId defined");
    }

    //Check if type is set to "participant"
    if (type === false){
        M.toast({html: 'You are not a participant. Close your room and join another or restart your Browser!'});
        throw new Error("Not a participant");
    }
} else {
    alert("Sorry, your browser does not support Web Storage...");
    throw new Error("No browser-support for Web Storage");
}

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/participant")
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .build();


async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
        JoinRoom(joinId);
        console.log("Joined SURVEYllance-Session with JoinId: " + joinId);
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

// Start the connection.
start();

// </editor-fold>

//<editor-fold desc="API-Methods">

//<editor-fold desc="Client">

//<editor-fold desc="Survey">

//TODO: Insert Survey-API-Methods
/**
 * Will be called by the server when the results of a survey change
 * @param {string} surveyId ID of the survey
 * @param {SurveyAnswer} answer the answer, which has changed
 */
connection.on("OnNewSurveyResult", (surveyId, answer) => {
    //Find Survey, maybe give each survey unique id?
    //Update Answer
});

/**
 * Will be called by the server when a survey is created
 * @param {Survey} survey The survey to display
 */
connection.on("OnNewSurvey", (survey) => {
    //Post new Survey
});

/**
 * Will be called by the server when a survey is closed
 * @param {string} surveyId ID of the survey
 */
connection.on("OnSurveyClose", (surveyId) => {
    //Delete Survey
});

/**
 * Will be called by the server when a survey is deleted
 * @param {string} surveyId The ID of the survey to delete
 */
connection.on("OnSurveyRemove", (surveyId) => {
    //Delete Survey
});

//</editor-fold>

//<editor-fold desc="Room">

/**
 * Will be called by the server when a room has been destroyed
 */
connection.on("OnRoomDestroy", () => {
    //Quit connection and leave page
});

//</editor-fold>

//TODO: Region Other

//</editor-fold>

//<editor-fold desc="Server">

//<editor-fold desc="Question">

/**
 * Ask a question
 * @param {Question} question the question to ask
 */
function AskQuestion(question) {
    try {
        connection.invoke("AskQuestion", question);
    } catch (err) {
        console.error(err);
    }
}

//</editor-fold>

//<editor-fold desc="Survey">

/**
 * Vote for an answer
 * @param {string} surveyId ID of the survey
 * @param {string} answerId ID of the answer
 * @returns {Survey} The survey with the answers visible
 */
async function Vote(surveyId, answerId) {
    try {
        return await connection.invoke("Vote", surveyId, answerId);
    } catch (err) {
        console.error(err);
    }
}

/**
 * Dismiss an survey
 * @param {string} surveyId ID of the survey
 * @returns {Survey} The survey with the answers visible
 */
async function Dismiss(surveyId) {
    try {
        return await connection.invoke("Dismiss", surveyId);
    } catch (err) {
        console.error(err);
    }
}

//</editor-fold>

//<editor-fold desc="Room">

/**
 * Join a room
 * @param {string} roomId ID of the room to join
 */
function JoinRoom(roomId) {
    try {
        connection.invoke("JoinRoom", roomId);
    } catch (err) {
        console.error(err);
    }
}

/**
 * Leave a room
 * Leave page
 */
function LeaveRoom() {
    try {
        connection.invoke("LeaveRoom");
        //TODO: Leave page
        //TODO: Remove localStorage
    } catch (err) {
        console.error(err);
    }
}
//</editor-fold>

//</editor-fold>

//</editor-fold>
