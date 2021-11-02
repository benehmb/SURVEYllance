/**
 * Survey-Object with reference to {@see HTMLElement}
 */

class SurveyDOM extends Survey {
    private surveyStatsNormalDiv;
    private surveyStatsExtendedDiv;
    private votesSum = 0;

    private domObject;

    get DomObject () {
        return this.domObject;
    }

    constructor(title, surveyAnswers, running) {
        let surveyAnswersListenable = null;
        surveyAnswers.forEach( (surveyAnswer, index) => {
            surveyAnswersListenable.push(new SurveyAnswerListenable(surveyAnswer, this.onVoteChange, index));
            this.votesSum += surveyAnswer.votes;
        })
        super(title, surveyAnswersListenable, running);
        this.domObject = this.createSurvey(super);
    }

    private onVoteChange(){
        super.answers.forEach( (surveyAnswer) => {
            this.votesSum += surveyAnswer.votes;
        });
        this.surveyStatsNormalDiv.innerHTML = "";

        let surveyNormalStatsDOM = this.createNormalStats(super.answers);
        surveyNormalStatsDOM.forEach((_surveyNormalStats) => {
            this.surveyStatsNormalDiv.appendChild(_surveyNormalStats);
        });

        this.surveyStatsExtendedDiv.innerHTML = "";
        let surveyStatsExtendedDOM = this.createExtendedStats(super.answers);
        surveyStatsExtendedDOM.forEach((_surveyExtendedStats) => {
            this.surveyStatsExtendedDiv.appendChild(_surveyExtendedStats);
        })
    }

    private createNormalStats(surveyAnswers){
        let surveyStatsNormal = new Array(0);
        surveyAnswers.forEach((surveyAnswer) =>{
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
            surveyAnswerProgress.style.width = (( surveyAnswer.votes / this.votesSum)*100).toFixed(0) + '%';
            surveyAnswerProgressCol.appendChild(surveyAnswerProgress);
        });
        return surveyStatsNormal;
    }

 private createExtendedStats (surveyAnswers){
     let surveyStatsExtended = new Array(0);
     let surveyExtendedAnswers = document.createElement('div');
     surveyExtendedAnswers.classList.add('row');

     surveyAnswers.forEach((surveyAnswer, index) =>{
         let surveyAnswerRow = document.createElement('div');
         surveyAnswerRow.classList.add('row');
         surveyStatsExtended.push(surveyAnswerRow);

         let surveyAnswerNameCol = document.createElement('div');
         surveyAnswerNameCol.classList.add('col', 's3');
         surveyAnswerRow.appendChild(surveyAnswerNameCol);

         let surveyAnswerNameReference = document.createElement('p');
         surveyAnswerNameReference.classList.add('truncate');
         surveyAnswerNameReference.innerHTML = (index + 1) + ': ' + (( surveyAnswer.votes / this.votesSum)*100).toFixed(0) + '%';
         surveyAnswerNameCol.appendChild(surveyAnswerNameReference);

         let surveyAnswerProgressCol = document.createElement('div');
         surveyAnswerProgressCol.classList.add('progress', 'col', 's9');
         surveyAnswerRow.appendChild(surveyAnswerProgressCol);

         let surveyAnswerProgress = document.createElement('div');
         surveyAnswerProgress.classList.add('determinate');
         // calculate percentage
         surveyAnswerProgress.style.width = (( surveyAnswer.votes / this.votesSum)*100).toFixed(0) + '%';
         surveyAnswerProgressCol.appendChild(surveyAnswerProgress);

         let surveyExtendedAnswerText = document.createElement('p');
         surveyExtendedAnswerText.innerHTML = (index+1) + ': ' + surveyAnswer.text;

         surveyExtendedAnswers.appendChild(surveyExtendedAnswerText);
     });
     surveyStatsExtended.push(surveyExtendedAnswers);

     return surveyStatsExtended;
 }
    //TODO Use running
    /**
     * Parse a {@see Survey}-Object into a {@see HTMLElement}
     * Example-Usage: {@code document.getElementById('survey-container').appendChild(createSurvey(<surveyObject>));}
     * @param {Survey} survey the survey to be parsed as a DOM-Object
     * @returns {HTMLDivElement} The element to be displayed
     */
    private createSurvey(survey){

        //<editor-fold desc="Create card">
        let surveyRow = document.createElement('div');
        surveyRow.classList.add('row', 'survey');

        let surveyCard = document.createElement('div');
        surveyCard.classList.add('card', 'hoverable');
        surveyRow.appendChild(surveyCard);

        let surveyCardContent = document.createElement('div');
        surveyCardContent.classList.add('card-content');
        surveyCardContent.setAttribute('onclick', 'toggleCard(this)');
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
        this.surveyStatsNormalDiv = surveyStatsNormal;

        let surveyStatsNormalDOM = this.createNormalStats(survey.answers);
        surveyStatsNormalDOM.forEach((_surveyNormalStats) => {
            surveyStatsNormal.appendChild(_surveyNormalStats);
        })

        //</editor-fold>

        //<editor-fold desc="Create SurveyStatsExtended element">
        let surveyStatsExtended = document.createElement('div');
        surveyStatsExtended.classList.add('surveyStatsExtended', 'hidden');
        surveyCardContent.appendChild(surveyStatsExtended);
        this.surveyStatsExtendedDiv = surveyStatsExtended;

        let surveyStatsExtendedDOM = this.createExtendedStats(survey.answers);
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
        surveyActions.appendChild(surveyActionClose);

        let surveyActionRemove = document.createElement('a');
        surveyActionRemove.setAttribute('onclick', 'removeElement(this)');
        surveyActionRemove.innerHTML = 'Remove survey';
        surveyActions.appendChild(surveyActionRemove);
        //</editor-fold>

        return surveyRow;
    }

}