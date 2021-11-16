class QuestionDOM extends Question{

    //TODO: More documentation
    #domObject;

    get domObject(){
        return this.#domObject;
    }

    /**
     * Constructor
     * @param {Question} question the question object to be displayed
     * @constructor
     */
    constructor(question) {
        //TODO: Backend writes variables in lowercase, but frontend writes them in uppercase.
        super(question.title);
        this.#domObject = this.#createQuestionDOM(question);
    }

    /**
     * Parse a {@see Question}-Object into a {@see HTMLElement}
     * @param {Question} question the question to be parsed
     * @return {HTMLDivElement} the element to be displayed
     */
    #createQuestionDOM(question) {
        let questionRow = document.createElement('div');
        questionRow.classList.add('row');

        let questionCard = document.createElement('div');
        questionCard.classList.add('card');
        questionRow.appendChild(questionCard);

        let questionCardContent = document.createElement('div');
        questionCardContent.classList.add('card-content');
        questionCard.appendChild(questionCardContent);

        let questionCardText = document.createElement('p');
        questionCardText.innerHTML = question.title;
        questionCardContent.appendChild(questionCardText);

        let questionCardAction = document.createElement('div');
        questionCardAction.classList.add('card-action');
        questionCard.appendChild(questionCardAction);

        let questionDoneAction = document.createElement('a');
        questionDoneAction.onclick = () => {
            this.closQuestion();
        };
        questionDoneAction.innerHTML = 'Done';
        questionCardAction.appendChild(questionDoneAction);

        return questionRow;
    }

    closQuestion() {
        this.#domObject.remove();
        RemoveQuestion(this.id);
    }

}