## Installing from zip release

1. Download the latest release from https://github.com/Nibato/nibmods-7dtd/releases

2. Disable Easy Anti-Cheat in the 7 Days to Die Launcher. Client-side harmony mods can **not** be used with EAC enabled.

3. Extract the contents of the zip file to your 7 Days to Die game directory (The default directory is usually `C:\Program Files (x86)\Steam\steamapps\common\7 Days To Die`)


## Building from Source

1. If not already installed, get Visual Studio 2019 and .NET Framework 4.6.1

2. Disable Easy Anti-Cheat in the 7 Days to Die Launcher. Client-side harmony mods can **not** be used with EAC enabled.

3. Clone the source code from [github](https://github.com/Nibato/nibmods-7dtd)

4. For each project, add the 7 Days to Die *Managed* folder to the project reference path. (Example: `C:\Program Files (x86)\Steam\steamapps\common\7 Days To Die\7DaysToDie_Data\Managed`)

5. These projects currently assume the game directory is `C:\Program Files (x86)\Steam\steamapps\common\7 Days To Die`. If your game directory lies elsewhere, you'll need update the debug and post-build events of each project to use the correct directory.