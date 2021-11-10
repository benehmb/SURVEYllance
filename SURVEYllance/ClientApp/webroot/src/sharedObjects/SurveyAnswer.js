/**
 * answers for a Survey
 *  @property {string} Id id to identify the answer from backend calls
 *  @property {string} Text text of the answer
 *  @property {number} Votes Number of votes for this answer
 */
class SurveyAnswer {

    //<editor-fold desc="Private properties">

    /**
     * id to identify the answer from backend calls
     * @type {string}
     * @private
     */
    #id;

    /**
     * text of the answer
     * @type {string}
     * @private
     */
    #text;

    /**
     * Number of votes. Can only be incremented (ath the moment) since votes can't be taken back
     * @type {number}
     * @private
     */
    #votes = 0;

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
     * Getter of {@link #text}
     * @returns {string}
     */
    get text() {
        return this.#text;
    }

    /**
     * Getter of {@link #votes}
     * @returns {number}
     */
    get votes() {
        return this.#votes;
    }

    /**
     * Setter of {@link #votes}
     * Check if votes have been increased
     * @param {string} value new value
     */
    set votes(value) {
        if (this.#votes < value) {
            this.#votes = value;
            //TODO: fire event?
        }
    }

    //</editor-fold>

    //<editor-fold desc="Constructor">

    /**
     * Constructor to create answers
     * @param {string} id id to identify the answer from backend calls
     * @param {String} text Name of the answer. NOT the number displayed before, which should be the Index
     * @param {Number} votes How many votes has this answer
     * @constructor
     */
    constructor(id, text, votes){
        this.#id = id;
        this.#text = text;
        this.#votes = votes;
    }

    //</editor-fold>

}