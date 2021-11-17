// Initiate duration of animation
const animTime = 250;

// Define max answers (TODO: export to settings)
const maxAnswers = 5;

// Create DOM Observer
var observeDOM = (function(){
    var MutationObserver = window.MutationObserver || window.WebKitMutationObserver;

    return function( obj, callback ){
        if( !obj || obj.nodeType !== 1 ) return;

        if( MutationObserver ){
            // define a new observer
            var mutationObserver = new MutationObserver(callback)

            // have the observer observe foo for changes in children
            mutationObserver.observe( obj, { childList:true, subtree:false })
            return mutationObserver
        }

        // browser support fallback
        else if( window.addEventListener ){
            obj.addEventListener('DOMNodeInserted', callback, false)
            obj.addEventListener('DOMNodeRemoved', callback, false)
        }
    }
})();

// Get Survey-Form
/**
 * Form element for new surveys
 * @type {HTMLFormElement}
 */
const surveyForm = document.getElementById("surveyForm");

// Observe surveyForm DOM Element:
observeDOM( surveyForm, function(m){
    m.forEach(record => {
        if(record.addedNodes.length>0){
            updateAnswers(false);
        }else if(record.removedNodes.length>0){
            updateAnswers(true);
        }
    });
});

// Get list of answers
let answers = surveyForm.getElementsByClassName("answer");

//TODO: Do not append it. Just return it
/**
 * Create a new answer and append it at the end
 */
function newAnswer(){
    if(answers.length >= maxAnswers){
        M.toast({html: "You have reached the maximum amount of " + maxAnswers + " answers!"});
        return;
    }
    let answerID = answers.length + 1;

    //create answer
    //<editor-fold desc="Creation of answerInput">
    let answerInput = document.createElement('input');
    answerInput.id = 'answer' + answerID;
    answerInput.type = "text";
    answerInput.required = true;
    answerInput.classList.add('validate');
    //</editor-fold>

    //<editor-fold desc="Creation of answerLabel">
    let answerLabel = document.createElement('label');
    answerLabel.setAttribute('for', answerInput.id);
    answerLabel.innerHTML = 'Answer ' + answerID;
    //</editor-fold>

    //<editor-fold desc="Creation of answerDeleteIcon">
    let answerDeleteIcon = document.createElement('i');
    answerDeleteIcon.classList.add('material-icons', 'col', 's1', 'red-text');
    answerDeleteIcon.innerHTML = 'delete_forever';
    answerDeleteIcon.setAttribute('onclick', 'deleteAnswer(this)');
    //</editor-fold>

    //<editor-fold desc="Creation of answerWrapper">
    let answerWrapper = document.createElement('div');
    answerWrapper.classList.add('input-field', 'col', 's11');
    answerWrapper.appendChild(answerInput);
    answerWrapper.appendChild(answerLabel);
    //</editor-fold>

    //<editor-fold desc="Creation of answer">
    let answer = document.createElement("div");
    answer.classList.add('row', 'valign-wrapper', 'answer');
    answer.appendChild(answerWrapper);
    answer.appendChild(answerDeleteIcon);
    //</editor-fold>

    //add answer
    surveyForm.appendChild(answer);

    //wait for animation to finish
    setTimeout(()=> answer.classList.add("animate"));
}

/**
 * Update {answers}-Object
 * Reindex answers if necessary
 * @param {boolean} reindex - reindex all answers
 */
function updateAnswers( reindex ){
    if (reindex) {
        console.log("Reindexing answers");
        for (let i = 0; i < answers.length; i++) {
            let answer = answers.item(i);
            let id = i + 1;

            let answerLabel = answer.getElementsByTagName('label').item(0);
            let answerInput = answer.getElementsByTagName('input').item(0);

            answerInput.id = "answer" + id;
            answerLabel.innerHTML = 'Answer ' + id;
            answerLabel.setAttribute('for', 'answer' + id);
        }
    }
    answers = surveyForm.getElementsByClassName("answer");
}

/**
 * Delete given answer
 * Displays toast if there are not enough answers
 * @param {Node & ParentNode} answer Material-Icon of answer
 */
function deleteAnswer(answer){
    if (answers.length <= 2) {
        M.toast({html: 'You must have at least two answers!'});
        return;
    }
    answer = answer.parentNode;
    answer.classList.remove('animate')
    setTimeout(() => {
        answer.remove();
    }, animTime);
}

//Check if user clicks outside of the popup and close it
window.onclick = function(event) {
    if (event.target === document.getElementById("newDialog")) {
        document.getElementById("newDialog").style.display = "none";
    }
}

//Take over form submit event.
surveyForm.addEventListener("submit", function (event) {
    event.preventDefault();
    //check if all answers are filled
    if(surveyForm.checkValidity())
        //submit form if all answers are filled
        sendData();
    else
        //display error if not
        surveyForm.reportValidity();
});

function sendData() {
    //Answers as Survey-Object-Array
    const surveyAnswers = new Array(0);
    //Fill surveyAnswers
    for (let i = 0; i < answers.length; i++) {
        surveyAnswers.push(new SurveyAnswer(answers[i].getElementsByTagName('input')[0].value));
        if (i>=2){
            //Delete answer, if there are more than 2
            answers[i].remove();
        }
    }
    //Create Survey-Object
    const survey = new Survey(surveyForm.getElementsByTagName('input')[0].value, surveyAnswers, false);
    console.log(survey);
    NewSurvey(survey);
    //Hide popup
    document.getElementById('newDialog').style.display = 'none';
    //Reset form
    surveyForm.reset();
}

function submitSurveyForm() {
    const domEvent = document.createEvent('Event')
    domEvent.initEvent('submit', true, true)
    surveyForm.dispatchEvent(domEvent)
}
