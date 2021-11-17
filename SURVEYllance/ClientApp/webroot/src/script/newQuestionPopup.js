/**
 * Form element for new questions
 * @type {HTMLFormElement}
 */
const questionForm = document.getElementById('questionForm');

//Check if user clicks outside of the popup and close it
window.onclick = function(event) {
    if (event.target === document.getElementById("newDialog")) {
        document.getElementById("newDialog").style.display = "none";
    }
}

//Take over form submit event.
questionForm.addEventListener("submit", function (event) {
    event.preventDefault();
    //check if all answers are filled
    if(questionForm.checkValidity())
        //submit form if all answers are filled
        sendData();
    else
        //display error if not
        questionForm.reportValidity();
});

function sendData() {
    //Answers as Survey-Object-Array
    const surveyAnswers = new Array(0);
    //Fill surveyAnswers
    const questionTxt = questionForm.getElementsByTagName('input')[0].value
    //Create Survey-Object
    const question = new Question(questionTxt);
    console.log(question);
    AskQuestion(question);
    //Hide popup
    document.getElementById('newDialog').style.display = 'none';
    //Reset form
    questionForm.reset();
}

function submitQuestionForm() {
    const domEvent = document.createEvent('Event')
    domEvent.initEvent('submit', true, true)
    questionForm.dispatchEvent(domEvent)
}

function closeQuestionForm() {
    document.getElementById('newDialog').style.display = 'none';
    questionForm.reset();
}