let joinId;

//<editor-fold desc="sessionStorageValues">
if (typeof(Storage) !== "undefined") {
    //Get value from localStorage (See readme.md for more details what to save in the localStorage)
    let type = sessionStorage.getItem("type");
    joinId = sessionStorage.getItem("joinId");

    //Check if type is set to "participant"
    if (type === true){
        M.toast({html: 'You are not a creator. Join a room or restart your Browser!'});
        throw new Error("Not a creator");
    }
} else {
    alert("Sorry, your browser does not support Web Storage...");
    throw new Error("No browser-support for Web Storage");
}
//</editor-fold>

// <editor-fold desc="Websocket-Connection">
// Connect to Websocket
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/creator")
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .build();
connection.logging = true;

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");

        //Get value from localStorage (See readme.md for more details what to save in the localStorage)
        let type = sessionStorage.getItem("type");
        joinId = sessionStorage.getItem("joinId");

        //Check if type and joinId are set
        if ((type === null || type === undefined) || (joinId === null || joinId === undefined)) {
            //If not, create a new room
            M.toast({html: 'Creating new room...'});
            joinId = await CreateRoom();
        } else {
            //If yes, join the room
            //Check if type is set to "creator"
            if (type !== true){
                M.toast({html: 'You are not a creator. Join a room or restart your Browser!'});
                throw new Error("Not a creator");
            }

            M.toast({html: 'You have already created a room. Trying to reconnect...'});

            CheckRoom(joinId).then(
                //If promise resolves, check result
                async function(result) {
                    if(result) {
                        M.toast({html: 'Room found. Joining...'});
                        JoinRoom(joinId);
                    }else {
                        M.toast({html: 'No room with this ID was found. Creating a new one...'});
                        joinId = await CreateRoom();
                    }
                },
                //If promise rejects, show error
                function (error) {
                    alert(error);
                    throw error;
                }
            );
            //Setting up the localStorage
            sessionStorage.clear();
            sessionStorage.setItem("type", true);
            sessionStorage.setItem("joinId", joinId);
            //TODO: Insert token here
        }

        console.log("New SURVEYllance-Session with JoinId: " + joinId);
        console.log("Go to '" + window.location.origin + "/join/" + encodeURIComponent(joinId) + "' to join it");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

// Start the connection.
start();

/**
 * Check if a room exists
 * @param joinId Id of the room
 * @return {Promise<unknown>} the promise
 * @constructor
 */
function CheckRoom(joinId) {

    //Create promise for the ajax request
    var promise = new Promise((resolve, reject) => {
        //Initialize the ajax request
        var roomRequest = new XMLHttpRequest();

        //Set the request method and url
        roomRequest.open("GET",  window.location.origin + "/rooms/"+joinId, true);

        //Handle the response
        roomRequest.onload = () =>{
            if (roomRequest.status === 200) {
                resolve(true);
            } else if (roomRequest.status === 404) {
                resolve(false);
            } else {
                reject(Error(roomRequest.statusText));
            }
        };

        //Handle errors
        roomRequest.onerror = () => {
            reject(Error("Network Error"));
        };

        //Send the request
        roomRequest.send();
    });

    //Return the promise
    return promise;

}

// </editor-fold>

const surveyContainer = document.getElementById('surveys');
const surveys = [];

const questionContainer = document.getElementById('questions');

//<editor-fold desc="API-Methods">

//<editor-fold desc="Client">

//<editor-fold desc="Question">

/**
 * Will be called by the server when a new question is created
 * @param {Question} question
 */
connection.on("OnNewQuestion", (question) => {
    let questionDOM = new QuestionDOM(question);
    questionContainer.appendChild(questionDOM.domObject);
});

//</editor-fold>

//<editor-fold desc="Survey">
/**
 * Will be called by the server when the results of a survey change
 * @param {string} surveyId ID of the survey
 * @param {SurveyAnswer} answer the answer, which has changed
 */
connection.on("OnNewSurveyResult", (surveyId, answer) => {
    surveys.find(survey => survey.id === surveyId).OnNewSurveyResult(answer);
});

/**
 * Will be called by the server when a survey is created
 * Only used on @see JoinRoom
 * @param {Survey} survey The survey to display
 */
connection.on("OnNewSurvey", (survey) => {
    let surveyDOM = new SurveyDOM(survey);
    surveys.push(surveyDOM);
    surveyContainer.appendChild(surveyDOM.domObject);
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
 */
async function NewSurvey(survey){
    try {
        let generatedSurvey = await connection.invoke("NewSurvey", survey);
        let surveyDOM = new SurveyDOM(generatedSurvey);
        surveys.push(surveyDOM);
        surveyContainer.appendChild(surveyDOM.domObject);
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
        surveys.forEach(survey => {
            if (survey.id === surveyId) {
                surveys.splice(surveys.indexOf(survey), 1);
            }
        });
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
