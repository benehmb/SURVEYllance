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
     * @param {string} title title of this question
     */
    constructor(title) {
        this.#title = title;
    }

    //</editor-fold>

    /**
     * This method overrides the default toJSON method, and returns object WITH the private properties, so they will be passed to the Backend
     * @return {{id: string, title: string}} Object with the private properties
     * @override
     */
    toJSON(){
        return {
            id: this.#id,
            title: this.#title
        }
    }


}