/**
 * Survey-Object with reference to {@see HTMLElement} and who has the ability to vote.
 * @property {HTMLElement} domObject - reference to {@see HTMLElement}
 */
class SurveyDOMVotable extends Survey {

    //<editor-fold desc="Private properties">

    /**
     * Reference to this survey in DOM
     * @private
     */
    #domObject;

    /**
     * Reference to survey form element
     * @type {HTMLFormElement}
     * @private
     */
    #surveyForm;

    //</editor-fold>

    //<editor-fold desc="Getter">

    /**
     * Getter for {@see #domObject}
     * @return {HTMLElement}
     */
    get domObject() {
        return this.#domObject;
    }

    //</editor-fold>

    //<editor-fold desc="Constructor">

    /**
     * Constructor to create a DOM-Bound Survey-Object
     * @param {Survey} survey the survey to bind to DOM
     */
    constructor(survey) {
        super(survey.title, survey.answers);
        this.id = survey.id;
        //create DOM-Object

        this.#domObject = this.#createSurvey(this);
    }

    //</editor-fold>

    //<editor-fold desc="Public methods">

    /**
     * Remove this survey from DOM
     */
    RemoveSurvey() {
        this.#domObject.remove();
    }

    SubmitSurvey() {
        //Get selected answer
        const selected = this.#surveyForm.querySelector('input[name="group"]:checked').value;
        //Submit answer
        Vote(this.id, selected);
    }

    DismissSurvey() {
        Dismiss(this.id);
    }

    //</editor-fold>

    //<editor-fold desc="Private methods">

    /**
     * Parse a {@see Survey}-Object into a {@see HTMLElement}
     * @param {Survey} survey the survey to be parsed as a DOM-Object
     * @returns {HTMLDivElement} The element to be displayed
     */
    #createSurvey(survey){

        //<editor-fold desc="Create card">
        let surveyRow = document.createElement('div');
        surveyRow.classList.add('row', 'survey');

        let surveyCard = document.createElement('div');
        surveyCard.classList.add('card');
        surveyRow.appendChild(surveyCard);

        let surveyCardContent = document.createElement('div');
        surveyCardContent.classList.add('card-content');
        surveyCard.appendChild(surveyCardContent);
        //</editor-fold>

        //<editor-fold desc="Create card title">
        let surveyCardTitle = document.createElement('div');
        surveyCardTitle.classList.add('card-title');
        surveyCardContent.appendChild(surveyCardTitle);

        let surveyCardTitleWrapper = document.createElement('div');
        surveyCardTitleWrapper.classList.add('valign-wrapper');
        surveyCardTitle.appendChild(surveyCardTitleWrapper);

        let surveyCardTitleIcon = document.createElement('i');
        surveyCardTitleIcon.classList.add('material-icons');
        surveyCardTitleIcon.innerHTML = 'help_outline';
        surveyCardTitleWrapper.appendChild(surveyCardTitleIcon);

        let surveyCardTitleText = document.createElement('p');
        surveyCardTitleText.innerHTML = survey.title.replace(/</g, "&lt;").replace(/>/g, "&gt;");
        surveyCardTitleWrapper.appendChild(surveyCardTitleText);
        //</editor-fold>

        //<editor-fold desc="Create Form element">
        this.#surveyForm = document.createElement('form');
        this.#surveyForm.classList.add('row');

        surveyCardContent.appendChild(this.#surveyForm);


        survey.answers.forEach((surveyAnswer) =>{
            let surveyAnswerRow = document.createElement('p');
            this.#surveyForm.appendChild(surveyAnswerRow);

            let surveyAnswerLabel = document.createElement('label');
            surveyAnswerRow.appendChild(surveyAnswerLabel);

            let surveyAnswerInput = document.createElement('input');
            surveyAnswerInput.classList.add('with-gap');
            surveyAnswerInput.name = 'group';
            surveyAnswerInput.type = 'radio';
            surveyAnswerInput.value = surveyAnswer.id;
            surveyAnswerLabel.appendChild(surveyAnswerInput);

            let surveyAnswerSpan = document.createElement('span');
            surveyAnswerSpan.classList.add('black-text');
            surveyAnswerSpan.innerHTML = surveyAnswer.text.replace(/</g, "&lt;").replace(/>/g, "&gt;");
            surveyAnswerLabel.appendChild(surveyAnswerSpan);
        });
        //</editor-fold>

        //<editor-fold desc="Create CardActions">
        let surveyActions = document.createElement('div');
        surveyActions.classList.add('card-action');
        surveyCard.appendChild(surveyActions);

        let surveyActionSubmit = document.createElement('a');
        surveyActionSubmit.innerHTML = 'Submit survey';
        surveyActionSubmit.onclick = () => {
            this.SubmitSurvey();
        };
        surveyActions.appendChild(surveyActionSubmit);

        let surveyActionDismiss = document.createElement('a');
        surveyActionDismiss.setAttribute('onclick', 'removeElement(this)');
        surveyActionDismiss.classList.add('red-text');
        surveyActionDismiss.innerHTML = 'Dismiss';
        surveyActionDismiss.onclick = () => {
            this.DismissSurvey();
        };
        surveyActions.appendChild(surveyActionDismiss);
        //</editor-fold>

        return surveyRow;
    }

    //</editor-fold>

}