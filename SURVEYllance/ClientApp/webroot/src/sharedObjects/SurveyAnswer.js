/**
 * Answers for a Survey
 * Each answer has
 *  - text: possible answer to the question
 *  - votes: how may votes has this answer
 */
class SurveyAnswer {
    text;
    votes;
    /**
     * Constructor to create answers
     * @param {String} text Name of the answer. NOT the number displayed before, which should be the Index
     * @param {Number} votes How many votes has this answer
     */
    constructor(text, votes){
        this.text = text;
        this.votes = votes;
    }
}