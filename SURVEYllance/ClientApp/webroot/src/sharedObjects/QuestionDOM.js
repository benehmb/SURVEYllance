class QuestionDOM extends Question{
    domObject;
    questionObj;

    get DomObject(){
        return this.domObject;
    }

    constructor(question) {
        super(question.title);
        this.questionObj = question;
        this.domObject = this.createQuestionDOM(question);
    }

    /**
     * Parse a {@see Question}-Object into a {@see HTMLElement}
     * @param {Question} question the question to be parsed
     * @return {HTMLDivElement} the element to be displayed
     */
    createQuestionDOM(question) {
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
        this.domObject.remove();
        connection.invoke("RemoveQuestion", this.questionObj);
    }

}