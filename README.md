# GLYPHS Archipelago

An archipelago implementation for GLYPHS by Vortex Bros.
Please report any bugs [here](https://github.com/BuffYoda21/ap-glyphs/issues)

## Installation Instructions

1. Download MelonLoader [here](https://github.com/LavaGang/MelonLoader/releases/tag/v0.7.0). I recommend using the cooresponding MelonLoader.Installer for your OS over the other options because it makes your life easier.
2. Run the installer, click on Glyphs, then "Install".
3. Download the mod from the [releases](https://github.com/BuffYoda21/ap-glyphs/releases) page.
4. Unzip ApGlyphs.zip and drop the contents in your game's `/Mods/` folder (this can be found by going to the game's settings in your steam library and selecting "Browse Local Files")
5. Launch Glyphs and wait until it lets you skip the intro then close the game. The game may take a moment to launch if this is your first time playing Glyphs with MelonLoader installed since it needs to generate a ton of files for you. How long this takes is entirely determined by the strength of your CPU.
6. In your `/UserData/` folder (next to your mods folder), there should now be a few new json files. Ignore `localInventory.json` as it is just to help the mod function. `Gamestate.json` keeps track of various events in your run and should be deleted when starting a new multiworld. Open `connectionConfig.json` and fill out all the neccesary information to connect to your archipelago room.
7. Launch Glyphs again and the blue archipelago logo should light up with fancy colors indicating you are connected! If it does not then double check the information in your `connectionConfig.json` and try again.
8. Create a new save file and enjoy!

## Building from source

1. Clone this repository
2. Create a new `/libs/` folder and drop copies of all the assemblies referenced in ApGlyphs.csproj (Found in `/MelonLoader/Il2CppAssemblies/` or `/MelonLoader/net6/` in your Glyphs instal folder)
3. Run `dotnet build` to build the mod
