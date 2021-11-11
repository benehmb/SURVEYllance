/**
 * Survey-Object represents survey and it's answers.
 *  @property {string} Id id to identify the survey from backend calls
 *  @property {string} Title title of the survey
 *  @property {Array<SurveyAnswer>} Answers Array of {@link SurveyAnswer}
 *  @property {boolean} IsClosed Is the survey still running or closed?
 */
class Survey {

    //<editor-fold desc="Private properties">

    /**
     * id to identify the survey from backend calls
     * @type {string}
     * @private
     */
    #id;

    /**
     * title of this survey
     * @type {string}
     */
    #title;

    /**
     * Possible answers
     * @type {Array<SurveyAnswer>}
     */
    #answers;

    /**
     * A survey can be closed, so the results will be displayed to anyone and it is not possible to vote anymore
     * @type {boolean}
     */
    #isClosed;

    //</editor-fold>

    //<editor-fold desc="Getter and Setter">

    /**
     * Getter of {@link #id}
     * @returns {string}
     */
    get id() {
        return this.#id;
    }

    /**
     * Getter of {@link #title}
     * @returns {string}
     */
    get title() {
        return this.#title;
    }

    /**
     * Getter of {@link #answers}
     * @returns {Array<SurveyAnswer>}
     */
    get answers() {
        return this.#answers;
    }

    /**
     * Setter of {@link #answers}
     * @param {Array<SurveyAnswer>} value New list of survey answers with updated values
     */
    set answers(value) {
        this.#answers = value;
    }

    /**
     * Getter of {@link #isClosed}
     * @returns {boolean}
     * @readonly
     */
    get isClosed() {
        return this.#isClosed;
    }

    /**
     * Setter of {@link #isClosed}
     * @param {boolean} value set survey closed
     */
    set isClosed(value) {
        if(!value)
            return;
        this.#isClosed = value;
        //TODO: Fire event
    }

    //</editor-fold>

    //<editor-fold desc="Constructor">

    /**
     * Constructor to create a survey
     * @param {string} id - the id of the survey
     * @param {string} title title of the survey, typically a question
     * @param {Array<SurveyAnswer>} surveyAnswers Array of answers. Must be of {@see surveyAnswers} Object
     * @param {boolean} isClosed Is the survey still running?
     * @constructor
     */
    constructor(id, title, surveyAnswers, isClosed) {
        this.#id = id;
        this.#title = title;
        this.#answers = surveyAnswers;
        this.#isClosed = isClosed;
    }

    //</editor-fold>

    //<editor-fold desc="Public methods">

    /**
     * Vote for an answer
     * Do not count if Survey is closed
     * @param {string} answerId ID of the answer to vote for
     */
    VoteForAnswer(answerId){
        if(this.#isClosed)
            return;
        //Vote for answer
        let answer = this.#answers.find(answer => answer.id === answerId);
        answer.votes++;
    }

    //</editor-fold>

}