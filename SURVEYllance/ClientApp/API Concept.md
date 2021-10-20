# General Questions

- ## Who can do what?
  - Room:
    - Can Close on Inactivity (Or time limit?)

  - User:
    - Can join Room
    - Can create Room

  - Creator:
    - Can create Survey
    - Can close Survey
    - Can delete survey
    - Can close room

  - Participant:
    - Can ask questions
    - Can vote on specific Survey
    - Can dismiss survey, if no opinion
    - Can Leve Room

- ## Who needs what?

  - Creator:
    - Need Questions
    - Need Results of Surveys
    - Notify if room is closed due to Inactivity

  - Participant:
    - Needs Status of Surveys
    - Needs Result of Surveys !IF VOTED, DISMISSED OR CLOSED!
    - Needs Surveys
    - Notify if room is closed due to Inactivity

---
# API:
- ## Server:
  - ### Room:
    - Create
    - Destroy
    - Join
    - Leave

  - ### Survey:
    - Create
    - Close
    - Destroy
    - Vote
    - Dismiss

  - ### Question:
    - Create
    - Destroy

- ## Client:
  - ### Participant:
    - Room:
      - Destroyed

    - Survey:
      - New Survey
      - Survey Closed
      - Update Results (If voted, dismissed or closed)
      - Destroy

  - ### Creator:
    - Room:
      - Destroyed

    - Survey:
      - Update Results

    - Question:
      - New Question

---
# Methods:
- ## Server:
  - Room:
    - Create
    - Destroy
    - Join
    - Leave

  - Survey:
    - Create
    - Destroy
    - Close
    - Vote
    - Dismiss

  - Question:
    - Create
    - Destroy

- ## Client:
  - ### User:
    - Room
      - Create Room
      - Join Room

  - ### Participant:
    - Room:
      - On Room Destroy
      - Leave
    - Survey:
      - On New
      - On Close
      - On New Result
      - On Destroy
      - Vote
      - Dismiss
    - Question:
      - Ask

  - ### Creator:
    - Room:
      - On Destroy
      - Destroy
    - Survey:
      - On New Result
      - New
      - Close
      - Destroy
    - Question:
      - On New
      - Destroy
