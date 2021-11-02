/**
 * Answers for a Survey
 * Each answer has
 *  - text: possible answer to the question
 *  - votes: how may votes has this answer
 *  Constructor needs methode to be called, when votes have been changed
 *  WARNING: naming in this class is different than usual. Its made to provide backwards compatibility for {@see Survey}
 */
class SurveyAnswerListenable {
    private _text;
    private _votes;
    private _eventHandler;
    index;

    get text(){
        return this._text;
    }

    get votes(){
        return this._votes;
    }

    set votes(value){
        this._votes = value;
        this._eventHandler();
    }

    /**
     * Constructor to create answers
     * @param {SurveyAnswer} surveyAnswer the answer to add listener
     * @param {function} eventHandler the methode to call when votes have been changed
     * @param {number} index of this answer
     */
    constructor(surveyAnswer, eventHandler, index){
        this._text = surveyAnswer.text;
        this._votes = surveyAnswer.votes;
        this._eventHandler = eventHandler;
        this.index = index;
    }
}