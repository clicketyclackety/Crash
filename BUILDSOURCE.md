# Build Crash! from source

These instructions will get you a copy of the project up and running on your
local machine for development and testing purposes.

## Prerequisites

* Git
  ([download](https://git-scm.com/downloads))
* Visual Studio 2022 (For Windows)
  ([download](https://visualstudio.microsoft.com/downloads/))
* Visual Studio Code (For Mac)
  ([download](https://code.visualstudio.com/Download))
* .NET Framework (4.8) & .NET Core (6.0) Developer Packs
  ([download](https://www.microsoft.com/net/download/visual-studio-sdks))
* Rhino
  ([download v7](https://www.rhino3d.com/download/rhino/7.0))
  ([download v8 (WIP)](https://discourse.mcneel.com/t/welcome-to-serengeti/9612))

## Getting Source & Build

1. Clone the repository. At the command prompt, enter the following command:

    ```console
    git clone --recursive https://github.com/clicketyclackety/crash.git
    ```

2. In Visual Studio, open `Crash\Crash.sln`.
3. Press F5 or Debug

## Installing & Uninstalling

Inside your build directory will be Crash.rhp, you will want to drag this into the Rhino window.
To uninstall navigate to `%appdata%\McNeel\Rhinoceros\packages\7.0` and delete the directory titled Crash.
