/**
 * Survey-Object with reference to {@see HTMLElement}
 * @property {HTMLElement} domObject - reference to {@see HTMLElement}
 */
class SurveyDOM extends Survey {

    //<editor-fold desc="Private properties">

    /**
     * Reference to SurveyStatsNormalDiv in DOM
     * @type {HTMLElement}
     */
    #surveyStatsNormalDiv;

    /**
     * Reference to SurveyStatsExtendedDiv in DOM
     * @type {HTMLElement}
     */
    #surveyStatsExtendedDiv;

    /**
     * Number of votes total
     * @type {number}
     */
    #votesSum = 0;

    /**
     * Reference to this survey in DOM
     */
    #domObject;

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
        //calculate votesSum
        this.answers.forEach(answer => {
            this.#votesSum += answer.votes;
        });
        //create DOM-Object
        this.#domObject = this.#createSurvey(this);

    }

    //</editor-fold>

    //<editor-fold desc="Public methods">

    /**
     * Updates the DOM-Object with the new data and update
     * (will also update the given answer in the list of answers)
     * @param {SurveyAnswer} answer
     * @constructor
     */
    OnNewSurveyResult(answer) {
        let answerIndex = this.answers.findIndex(value => value.id === answer.id);
        this.answers[answerIndex] = answer;
        this.#votesSum = 0;
        this.answers.forEach((surveyAnswer) => {
            this.#votesSum += surveyAnswer.votes;
        });
        this.#surveyStatsNormalDiv.innerHTML = "";

        let surveyNormalStatsDOM = this.#createNormalStats(this.answers);
        surveyNormalStatsDOM.forEach((_surveyNormalStats) => {
            this.#surveyStatsNormalDiv.appendChild(_surveyNormalStats);
        });

        this.#surveyStatsExtendedDiv.innerHTML = "";
        let surveyStatsExtendedDOM = this.#createExtendedStats(this.answers);
        surveyStatsExtendedDOM.forEach((_surveyExtendedStats) => {
            this.#surveyStatsExtendedDiv.appendChild(_surveyExtendedStats);
        })
    }

    /**
     * Remove this survey from DOM and notify the server
     */
    RemoveSurvey() {
        this.#domObject.remove();
        RemoveSurvey(this.id);
    }

    /**
     * Close this survey and notify the server
     */
    CloseSurvey() {
        CloseSurvey(this.id);
        //TODO: Do something to display that the survey is closed
    }

    /**
     * Toggle between normalSurveyStats and ExtendedSurveyStats
     */
    ToggleSurvey() {
        //TODO: Replace toggle with fancy animation
        this.#surveyStatsNormalDiv.classList.toggle('hidden');
        this.#surveyStatsExtendedDiv.classList.toggle('hidden');
    }

    //</editor-fold>

    //<editor-fold desc="Private methods">

    /**
     * Creates the DOM-Object for the SurveyStats
     * @param {Array<SurveyAnswer>} surveyAnswers list of answers
     * @return {Array<HTMLElement>} Array of HTML-Element with each answer
     */
    #createNormalStats(surveyAnswers) {
        let surveyStatsNormal = new Array(0);
        surveyAnswers.forEach((surveyAnswer) => {
            let surveyAnswerRow = document.createElement('div');
            surveyAnswerRow.classList.add('row');
            surveyStatsNormal.push(surveyAnswerRow);

            let surveyAnswerNameCol = document.createElement('div');
            surveyAnswerNameCol.classList.add('col', 's4');
            surveyAnswerRow.appendChild(surveyAnswerNameCol);

            let surveyAnswerNameText = document.createElement('p');
            surveyAnswerNameText.classList.add('truncate');
            surveyAnswerNameText.innerHTML = surveyAnswer.text;
            surveyAnswerNameCol.appendChild(surveyAnswerNameText);

            let surveyAnswerProgressCol = document.createElement('div');
            surveyAnswerProgressCol.classList.add('progress', 'col', 's7', 'offset-s1');
            surveyAnswerRow.appendChild(surveyAnswerProgressCol);


            let surveyAnswerProgress = document.createElement('div');
            surveyAnswerProgress.classList.add('determinate');
            // calculate percentage
            surveyAnswerProgress.style.width = ((surveyAnswer.votes / this.#votesSum) * 100).toFixed(0) + '%';
            surveyAnswerProgressCol.appendChild(surveyAnswerProgress);
        });
        return surveyStatsNormal;
    }

    /**
     * Creates the DOM-Object for the ExtendedSurveyStats
     * @param {Array<SurveyAnswer>} surveyAnswers list of answers
     * @return {Array<HTMLElement>} HTML-Element with the ExtendedSurveyStats
     */
    #createExtendedStats(surveyAnswers) {
        let surveyStatsExtended = new Array(0);
        let surveyExtendedAnswers = document.createElement('div');
        surveyExtendedAnswers.classList.add('row');

        surveyAnswers.forEach((surveyAnswer, index) => {
            let surveyAnswerRow = document.createElement('div');
            surveyAnswerRow.classList.add('row');
            surveyStatsExtended.push(surveyAnswerRow);

            let surveyAnswerNameCol = document.createElement('div');
            surveyAnswerNameCol.classList.add('col', 's3');
            surveyAnswerRow.appendChild(surveyAnswerNameCol);

            let surveyAnswerNameReference = document.createElement('p');
            surveyAnswerNameReference.classList.add('truncate');
            surveyAnswerNameReference.innerHTML = (index + 1) + ': ' + ((surveyAnswer.votes / this.#votesSum) * 100).toFixed(0) + '%';
            surveyAnswerNameCol.appendChild(surveyAnswerNameReference);

            let surveyAnswerProgressCol = document.createElement('div');
            surveyAnswerProgressCol.classList.add('progress', 'col', 's9');
            surveyAnswerRow.appendChild(surveyAnswerProgressCol);

            let surveyAnswerProgress = document.createElement('div');
            surveyAnswerProgress.classList.add('determinate');
            // calculate percentage
            surveyAnswerProgress.style.width = ((surveyAnswer.votes / this.#votesSum) * 100).toFixed(0) + '%';
            surveyAnswerProgressCol.appendChild(surveyAnswerProgress);

            let surveyExtendedAnswerText = document.createElement('p');
            surveyExtendedAnswerText.innerHTML = (index + 1) + ': ' + surveyAnswer.text;

            surveyExtendedAnswers.appendChild(surveyExtendedAnswerText);
        });
        surveyStatsExtended.push(surveyExtendedAnswers);

        return surveyStatsExtended;
    }

    //TODO Use running
    /**
     * Parse a {@see Survey}-Object into a {@see HTMLElement}
     * Example-Usage: {@code document.getElementById('survey-container').appendChild(createSurvey(<surveyObject>));}
     * @param {SurveyDOM} survey the survey to be parsed as a DOM-Object
     * @returns {HTMLDivElement} The element to be displayed
     */
    #createSurvey(survey) {

        //<editor-fold desc="Create card">
        let surveyRow = document.createElement('div');
        surveyRow.classList.add('row', 'survey');

        let surveyCard = document.createElement('div');
        surveyCard.classList.add('card', 'hoverable');
        surveyRow.appendChild(surveyCard);

        let surveyCardContent = document.createElement('div');
        surveyCardContent.classList.add('card-content');
        //TODO: Change to surveyCardContent.onclick...
        surveyCardContent.onclick = () => {
            this.ToggleSurvey();
        };
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
        surveyCardTitleText.innerHTML = survey.title;
        surveyCardTitleWrapper.appendChild(surveyCardTitleText);
        //</editor-fold>

        //<editor-fold desc="Create SurveyStatsNormal element">
        let surveyStatsNormal = document.createElement('div');
        surveyStatsNormal.classList.add('surveyStatsNormal');
        surveyCardContent.appendChild(surveyStatsNormal);
        this.#surveyStatsNormalDiv = surveyStatsNormal;

        let surveyStatsNormalDOM = this.#createNormalStats(survey.answers);
        surveyStatsNormalDOM.forEach((_surveyNormalStats) => {
            surveyStatsNormal.appendChild(_surveyNormalStats);
        })

        //</editor-fold>

        //<editor-fold desc="Create SurveyStatsExtended element">
        let surveyStatsExtended = document.createElement('div');
        surveyStatsExtended.classList.add('surveyStatsExtended', 'hidden');
        surveyCardContent.appendChild(surveyStatsExtended);
        this.#surveyStatsExtendedDiv = surveyStatsExtended;

        let surveyStatsExtendedDOM = this.#createExtendedStats(survey.answers);
        surveyStatsExtendedDOM.forEach((_surveyExtendedStats) => {
            surveyStatsExtended.appendChild(_surveyExtendedStats);
        })

        //</editor-fold>

        //<editor-fold desc="Create CardActions">
        let surveyActions = document.createElement('div');
        surveyActions.classList.add('card-action');
        surveyCard.appendChild(surveyActions);

        let surveyActionClose = document.createElement('a');
        surveyActionClose.innerHTML = 'Close survey';
        surveyActionClose.onclick = () => {
            this.CloseSurvey();
        };
        surveyActions.appendChild(surveyActionClose);

        let surveyActionRemove = document.createElement('a');
        surveyActionRemove.innerHTML = 'Remove survey';
        surveyActionRemove.onclick = () => {
            this.RemoveSurvey();
        };
        surveyActions.appendChild(surveyActionRemove);
        //</editor-fold>

        return surveyRow;
    }

    //</editor-fold>

}