Supplies a loadout of your choosing at the start of a run. Through the in game UI or config file, you can select items and their quantity to be spawned at the start of the first round.

Only necessary for the host.

## In Game UI

You can access the in game UI from either the lobby screen or pause menu.

All items are set to 0 by default. Set an item quantity to -1 to spawn one for each player. A number > 0 will spawn exactly that many.

**FOR SINGLEPLAYER (Not solo lobby):** Singleplayer can only access the UI from the pause menu. This means you must start a run, select your loadout, then restart the run to apply changes. Or just edit the config file.

## Config File

The mod config file will be named "shaboingboings.Loadout.cfg" located in the REPO/Bepinex/config folder. This config file will be changed by the in game UI, so any changes made in the file or UI will be reflected in the other.

### Config syntax: 

 - Item List: Separate each item with a semicolon. By default, one copy will be spawned for each player in the lobby. To spawn custom amounts, add a hash symbol followed by quantity.

 - Preset List: Separate each preset with a vertical bar ```|```. Each preset should use the item list syntax above.

Example: 
```Item Gun Handgun;Item Gun Shotgun#2;Item Upgrade Player Grab Range#10```

This config will spawn one handgun for each player, 2 shotguns, and 10 grab range upgrades.

```Item Gun Handgun|Item Gun Handgun;Item Gun Shotgun#2;Item Upgrade Player Grab Range#10|```

This config will create 2 presets, one with a handgun for each player, the other a copy of the item list above.

A list of vanilla item names are supplied in the config file. Modded items will appear in the game UI exactly as they should be typed.

## Requirements

[BepInEx](https://github.com/bepinex/bepinex) - Releases are on the right side. Download the zip

[MenuLib](https://thunderstore.io/c/repo/p/nickklmao/MenuLib/)

## Installation

Mod manager: Click download and let it do the work.

Manual: Extract file contents into your REPO folder

### **Extra**

Here's the [Github](https://github.com/dubtcs/repo-givegun) if you want to check out the code. Viewing mod source is a great way to learn. Thanks for downloading and please let me know of any questions, issues, or additional features you'd like.

 - shaboingboings
