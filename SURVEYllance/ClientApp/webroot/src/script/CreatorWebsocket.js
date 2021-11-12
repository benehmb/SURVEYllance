let joinId;

// <editor-fold desc="Websocket-Connection">
// Connect to Websocket
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/creator")
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .build();

//TODO: At the moment we can only create a room, not join one.
async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
        joinId = await CreateRoom();
        console.log("New SURVEYllance-Session with JoinId: " + joinId);
        console.log("Go to '" + window.location.origin + "/join/" + encodeURIComponent(joinId) + "' to join it");
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

//<editor-fold desc="Question">

/**
 * Will be called by the server when a new question is created
 * @param {Question} question
 */
connection.on("OnNewQuestion", (question) => {
    let questionDOM = new QuestionDOM(question);
    document.getElementById('questions').appendChild(questionDOM.domObject);
});

//</editor-fold>

//<editor-fold desc="Survey">
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
 * Only used on @see JoinRoom
 * @param {Survey} survey The survey to display
 */
connection.on("OnNewSurvey", (survey) => {
    //Post new Survey
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
 * Remove a question
 * @param {string} questionId ID of the question to remove
 */
function RemoveQuestion(questionId) {
    try {
        connection.invoke("RemoveQuestion", questionId);
    } catch (err) {
        console.error(err);
    }
}
//</editor-fold>

//<editor-fold desc="Survey">
/**
 * Create a new survey
 * @param {Survey} survey The survey to create
 * @deprecated
 */
function NewSurvey(survey){
    try {
        connection.invoke("NewSurvey", survey);
    } catch (err) {
        console.error(err);
    }
}

/**
 * Create a new survey
 * @param {string} title title of the survey
 * @param {Array<string>} answers List of answers
 * @return {Survey} Survey object, created by the server
 */
async function NewSurvey(title, answers) {
    try {
        return await connection.invoke("NewSurvey", title, answers);
    } catch (err) {
        console.error(err);
    }
}

/**
 * Close a survey, so no more votes can be added
 * @param {string} surveyId ID of the survey to close
 */
function CloseSurvey(surveyId) {
    try {
        connection.invoke("CloseSurvey", surveyId);
    } catch (err) {
        console.error(err);
    }
}

/**
 * Remove a survey
 * @param {string} surveyId ID of the survey to remove
 * @constructor
 */
function RemoveSurvey(surveyId) {
    try {
        connection.invoke("RemoveSurvey", surveyId);
    } catch (err) {
        console.error(err);
    }
}
//</editor-fold>

//<editor-fold desc="Room">
/**
 * Create a new room
 * @return {string} ID of the room
 */
async function CreateRoom() {
    try {
        return await connection.invoke("CreateRoom");
    } catch (err) {
        console.error(err);
    }
}

/**
 * Join a room
 * @param {string} roomId ID of the room to join
 */
async function JoinRoom(roomId) {
    try {
        await connection.invoke("JoinRoom", roomId);
    } catch (err) {
        console.error(err);
    }
}

/**
 * Destroy a room
 * Leave page
 */
function DestroyRoom() {
    try {
        connection.invoke("DestroyRoom");
        //TODO: Leave page
        //TODO: Remove localStorage
    } catch (err) {
        console.error(err);
    }
}
//</editor-fold>

//</editor-fold>

//</editor-fold>
