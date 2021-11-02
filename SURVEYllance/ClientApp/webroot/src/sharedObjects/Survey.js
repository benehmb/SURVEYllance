/**
 * Survey-Object represents survey and it's answers.
 * Can be parsed to a DOM-Object with {@see createSurvey}
 * Each survey has:
 *  - title: usually a question, to be answered
 *  - answers: an {@see Array} of {@see SurveyAnswer}, which contains possible answers
 *  - running: is the survey still running or closed?
 */
class Survey {
    title;
    answers;
    running;
    /**
     * Constructor to create a survey
     * @param {String} title Title of the survey, typically a question
     * @param {Array<SurveyAnswer>} surveyAnswers Array of answers. Must be of {@see surveyAnswers} Object
     * @param {Boolean} running Is the survey still running?
     */
    constructor(title, surveyAnswers, running) {
        this.title = title;
        this.answers = surveyAnswers;
        this.running = running;
    }
}