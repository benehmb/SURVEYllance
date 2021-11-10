/**
 * Question-Object represents questions that might appear
 * @property {string} Id id to identify the question from backend calls
 * @property {string} Text The title of the question
 */
class Question{

    //<editor-fold desc="Private properties">

    /**
     * id to identify the question from backend calls
     * @type {string}
     * @private
     */
    #id;

    /**
     * title of the question
     * @type {string}
     * @private
     */
    #title;

    //</editor-fold>

    //<editor-fold desc="Getter and Setter">

    /**
     * Getter for {@link #id}
     * @type {string}
     */
    get id(){
        return this.#id;
    }

    /**
     * Getter for {@link #title}
     * @type {string}
     */
    get title(){
        return this.#title;
    }

    //</editor-fold>

    //<editor-fold desc="Constructor">

    /**
     * Constructor to create a question
     * @param {string} id id to identify the question from backend calls
     * @param {string} title title of this question
     */
    constructor(id, title) {
        this.#id = id;
        this.#title = title;
    }

    //</editor-fold>

}