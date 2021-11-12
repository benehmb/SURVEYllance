# SURVEYllance

## About this project:

This project was created for classrooms, online meetings or any other form of meeting.

Its really simple to use. The creator, usually the person who presents, creates a new room. Anyone else in the Meeting can join this room by it's `Join-ID` aka `Room-Code`.

Now the creator has the possibility to create survey. It's the simplest type of survey: You can have one Question and a limited amount of questions which *can be specified in the config* (sadly not implemented yet). Any participant will get this survey and can vote for it's answer or dismiss if the participant has no opinion about this or doesn't want to give it's opinion.

Additional every participant has the ability to ask a question. This question will be displayed to the creator.

That's it. It' completely anonymous, no login required, no names to give and the Room will be destroyed and any data will be deleted after the creator has left it.



## How to use

1. Go to [SURVEYllance.HackerMB.com](https://SURVEYllance.hackermb.com/)
2. Usage:
   1. If you are a presenter, click on  `CREATE NEW ROOM` and thats it
   2. If you are a Participant, click on `JOIN ROOM` and enter the Room-Code, given by the presenter



## How to setup

None of the following has been tested, since there is no complete working project an i have no use to deploy something that isn't working. Will be tested and completed after anything works basically.

### Docker

This project will be served as a Docker-Container, so if you are using docker, just use the Docker-File

// Additional documentation coming soon

### Other

This project was written in .NET Core, so if you have installed .NET 5.0 ore above, you can use the following commands inside the project-root: 

```bash
dotnet restore "SURVEYllance/SURVEYllance.csproj"
dotnet build "SURVEYllance.csproj" -c Release -o /app/build
dotnet publish "SURVEYllance.csproj" -c Release -o /app/publish
. /app/publish/SURVEYllance.dll
```

## Settings

// Nothing here at the moment

## API

You can find a concept of the API in [API Concept.md](./SURVEYllance/API%20Concept.md)

## I am a developer and want to help

I have a long TODO-List here [TODO.txt](./SURVEYllance/TODO.txt), so feel free to work on it. Additionally i have some `//TODO` comments. As always, you can refactor anything to make more readable or more efficient. And if there is something else, feel free to do is :)

## Setting up environment

I use [Rider](https://www.jetbrains.com/rider/) for the pack-end-project and [WebStorm](https://www.jetbrains.com/webstorm/) for the front-end-project. But sometimes i need to use [VisualStudio](https://visualstudio.microsoft.com/), since most C# instructions are made for it.

Note for Rider: I added `//FIXME`-comments:

1. File | Settings | Editor | TODO
2. Under `Patterns:`, click the `+`
3. `Edit Pattern`-Window appears
4. Enter `\bfixme\b.*` as Pattern
5. Optional: Change color

![fixme-settings](Images\fixme-settings.png)

(Also described [Here]([Are FIXME comments supported in Rider? Can only see TODO items – Rider Support | JetBrains](https://rider-support.jetbrains.com/hc/en-us/community/posts/360009890900-Are-FIXME-comments-supported-in-Rider-Can-only-see-TODO-items)))

For the API between front-end and back-end, i use [ASP.NET with SignalR ](https://dotnet.microsoft.com/apps/aspnet/signalr), so if you need some Documentation, there it is: [ASP.NET Core SignalR Documentation](https://docs.microsoft.com/de-de/aspnet/core/signalr/introduction)

On the front-end side, i use [Materialize](https://materializecss.com/) for the UI, and also SignalR since it's the API. It has been installed with [npm](https://www.npmjs.com/). Usually you shouldn't need it, but in case you need some more Libraries or need to update SignalR, you need to Install [NodeJS](https://nodejs.org/) along with npm. Npm installs it's modules in `<front-end project-root>/node_modules`, so if you install some, you need to add them to the `<front-end project-root>/cpLib.ps1` file and execute it. Beware of the relative paths in this file, so use the project-root as working directory.

That's it. The Run-Configuration should already be there, so just click play in your IDE and there you go. If it won`t work, you can use the Script from [How to setup > Other](#Other)

## Documentation

This section is for anything, that is related to documentation, and has no place in any other File

### Browser-Session-Storage

What is stored in the session-storage and what does it mean

| Key            | Value                                                        | Description                                                  |
| -------------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| `type`         | `bool: true` - The Person is a `creator` of this room<br />`bool: false` - The Person is a `participant` of this room | Used to determinate if a User is a `creator` or a `participant`<br />If you just change the URL from `pareticipant.html` to `creator.html` this is the first hurdle to overcome |
| `joinId`       | `string` - ID                                                | ID of the room you are in. Used to reconnect if you reload the page or accidentally close the Tab |
| `creatorToken` | `string` - Token                                             | Used to verify, that you are the creator of this room. on reloading/reconnecting<br />**Not used at the moment, since we have no token-generation yet** |
