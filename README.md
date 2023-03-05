<a name="readme-top"></a>

<!-- PROJECT SHIELDS -->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<!-- PROJECT LOGO -->
<div align="center">
  <a href="https://github.com/clicketyclackety/Crash">
    <img src="Logo.png" alt="Logo" width="400" height="300">
  </a>

  <p align="center">
    A multi-user collaborative environment for Rhino
    <br />
    <a href="https://rhinocrash.notion.site/CRASH-6fdc9286ff33490487c6585b2f17c33d">User Guide</A>
    ·
    <a href="https://github.com/clicketyclackety/Crash/issues">Report Bug</a>
    ·
    <a href="https://github.com/clicketyclackety/Crash/issues">Request Feature</a>
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#baby-poweruser-getting-started">Poweruser Getting Started</a>
    </li>
    <li>
      <a href="#man_technologist-woman_technologist-developer-getting-started">Developer Getting Started</a>
    </li>
    <li><a href="#workflow-overview">Workflow Overview</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project
This project has been completed as part of the TT AEC Hackathon 2022 - New York. This plugin/application allows users to collaborate on a single central Rhino model. The Team Members for this awesome project are (in alphabetical order):
* [Callum Sykes](https://www.linkedin.com/in/callumsykes/)
* [Curtis Wensley](https://www.linkedin.com/in/cwensley/)
* [Erika Santos](https://www.linkedin.com/in/erikasantosr/)
* [Lukas Fuhrimann](https://www.linkedin.com/in/lfuhrimann/)
* [Morteza Karimi](https://github.com/karimi)
* [Moustafa El-Sawy](https://www.linkedin.com/in/moustafakelsawy/)
* [Russell Feathers](https://www.linkedin.com/in/russell-feathers/)
<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Built With
* [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
* [SignalR](https://learn.microsoft.com/en-us/aspnet/signalr/overview/getting-started/introduction-to-signalr)
* [SQLite](https://www.sqlite.org/index.html)
<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- POWERUSER GETTING STARTED -->
## :baby: Poweruser Getting Started
Thanks for checking out CRASH! Please follow the steps below to get started in no time! Please make sure you have all the <a href="#prerequisites">Prerequisites</a> to have a smooth and fun experience!

### Prerequisites
You will need the following libraries and/or software installed before getting to the fun!
* [Rhino 7.21+](https://www.rhino3d.com/download/)

### Installing CRASH from YAK
1. Launch Rhino 7
2. Type in PackageManager or go to Tools --> Package Manager
3. Search for Crash and press Install.
4. Close and Re-launch Rhino 7.

### Using Crash
To host a new shared model:
1. Type `StartSharedModel` command in Rhino.
2. Enter your name when prompted.
3. Specify an open port on your machine to run the server
4. Others can join the session using url `<your_ip_address>:<port>`

![Alt Text](https://media.giphy.com/media/oNuY0wsiDV5XFmYuNw/giphy.gif)

To Join a shared model:
1. Type `OpenSharedModel` command in Rhino.
2. Enter your name when prompted.
3. Enter the server URL from step 4 above.

You're now connected in a collaborative session. To commit your changes to the central model use the `Release` command.

<!-- DEVELOPER GETTING STARTED -->
## :man_technologist: :woman_technologist: Developer Getting Started
Thanks again for checking out CRASH! Please follow the steps below to get started and diving into the code in no time! Please sure sure you have all the <a href="#prerequisites-1">Prerequisites</a> to have a smooth, unbuggy and fun experience!

### Prerequisites
You will need the following libraries and/or software installed before getting to the fun!
* [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)
* [.NET Core 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* [Rhino 7.21+](https://www.rhino3d.com/download/)
* [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

### Prerequisites (MacOS)
You can also build and debug on MacOS using VS Code!
* [Visual Studio Code](https://code.visualstudio.com/)
* [Rhino 8 WIP](https://www.rhino3d.com/download/rhino/wip) is required on ARM machines.

### Getting Source

Clone the repo
   ```sh
   git clone https://github.com/clicketyclackety/Crash.git
   ```

### Building

#### Windows
Open Crash repository in Visual Studio:
  1. Set Crash as startup project.
  2. Build solution.
  3. Drag and drop `Crash\Crash\bin\Debug\net48\Crash.rhp` into an open Rhino window.
  4. Re-open Rhino.
  5. Happy debugging.

#### MacOS
Open Crash repository in VS Code run build tasks `⇧⌘B` in this order:
  1. `buid-plugin`
  2. `build-server`
  3. `publish-server`
From `Run and Debug` tab run `Run Rhino 8 WIP`

Rhino will launch in debug mode.

## Docker

Navigate to the Crash directory root. Run build to create the image.
```powershell
docker build -t "crash.server" .
```
Use docker run to start the image
```powershell
docker run -d -p 8080:80 --name crashy01 "crash.server"
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- WORKFLOW EXAMPLES -->
## Workflow Overview
Crash works by allowing back & forth communication of clients with a central server to either send changes or receive changes. The server keeps record of a list of objects along with relevant attributes to allow the functionality required. One important distinction here is that the database/server will hold two types of objects; baked and ghost objects. Baked objects are drawn into the Rhino model while Non-Baked (called ghost here) objects are "Pipeline" objects. Communication between the client and database occurs in the form of invoking end points on either side and sending over "Change" objects that contain all the required information.

The following steps show a complete workflow of how the system works. For this example, there are 3 users (Bob, John, Mary) working on a central model called "NYC Building 5".
1. Bob has a current Rhino model. He realizes the deadline is coming up and will need help from John & Mary.
2. Bob initiates a shared model using Crash. This will create a server on his machine locally. Initially the server database is empty and Bob is the initiator so his machine would send all the current Rhino Geometry in his file to the server as a List of Change objects and invoke the appropriate command on the server.
3. The server launches, receives data from the first initialization & populates its database with the list of objects received.
4. John launches Rhino with an empty file. John then starts up Crash and selects to link to "NYC Building 5". The server instantly sends him all the list of objects in the database and invokes client side end points to re-create these objects in his model (both baked and ghost objects).
5. Johns starts to draw new geometrical objects. After every action he performs (Add/Deleting/Updating), the client (John) invokes the appropriate command on the server and sends the required information as Change objects. Currently all new objects are considered ghost objects. All users see these new ghost objects in their views but are unable to select them or modify them.
6. John is done creating new geometry and would like to "commit" these changes to other users. He presses the "Im done!" button. This invokes a command on the server to convert all objects owned/created by John and change their status from ghost to baked objects. This change is then pushed to all clients and they will see these objects turn into a baked object.
7. Mary launches Rhino with an empty file. Mary then starts up Crash and selects to link to "NYC Building 5". The server instantly sends her all the list of objects in the database and invokes client side end points to re-create these objects in her model (both baked and ghost objects).
8. Mary decides to select and delete one of the objects she sees. This will invoke the delete command on the server side and update the database.
9. After the database is updated (on this deleted baked object), it invokes the delete function on all clients to remove this object from their Rhino model.
10. All users now have the same objects in their model (baked and ghost objects).
11. John selects an element and is thinking of what change he needs to do this object. As soon as he selects this object, his client machine would send the server and invoke the command to modify this object and mark it as locked. This would not allow any other user to select it until he presses "Im done!" button.
<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ROADMAP -->
## Roadmap
- [X] Local web server 
- [X] Rhino plugin (window)
- [X] Deploy plugin to YAK
- [X] Deploy webserver (Azure)
- [ ] Version control
- [ ] CI
    - [ ] Unit tests
    - [ ] Push to deploy
- [ ] Expand Supported types
    - [ ] Layers
    - [ ] Object attributes
    - [ ] Document settings
- [ ] Authorization 
    - [ ] Rhino Accounts integration
    - [ ] Permissions and access management

See the [open issues](https://github.com/clicketyclackety/Crash/issues) for a full list of proposed features (and known issues).
<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTRIBUTING -->
## Contributing
[Please see contribution guide](CONTRIBUTING.md)

<!-- LICENSE -->
## License
Distributed under the MIT License. See `LICENSE.txt` for more information.
<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ACKNOWLEDGMENTS -->
## Acknowledgments
Big thanks to AEC Tech 2022 for arranging this event! Also we would like to thank McNeel for all their awesome work! This project has been a great collaboration of several great minds. Please check out other hackathon projects and future hackathon events hosted by [AECTech](https://www.aectech.us/).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/clicketyclackety/Crash.svg?style=for-the-badge
[contributors-url]: https://github.com/clicketyclackety/Crash/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/clicketyclackety/Crash.svg?style=for-the-badge
[forks-url]: https://github.com/clicketyclackety/Crash/network/members
[stars-shield]: https://img.shields.io/github/stars/clicketyclackety/Crash.svg?style=for-the-badge
[stars-url]: https://github.com/clicketyclackety/Crash/stargazers
[issues-shield]: https://img.shields.io/github/issues/clicketyclackety/Crash.svg?style=for-the-badge
[issues-url]: https://github.com/clicketyclackety/Crash/issues
[license-shield]: https://img.shields.io/github/license/clicketyclackety/Crash.svg?style=for-the-badge
[license-url]: https://github.com/clicketyclackety/Crash/blob/master/LICENSE.txt
[product-screenshot]: images/screenshot.png
