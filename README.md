# Loadout
A mod for REPO that spawns a loadout for the lobby on the starting round.
## Install
### Requirements

 - BepInEx
 - MenuLib (for v1.3.0+)

Download and extract all of the contents into your ```REPO/bepinex/plugins``` folder.
## Config
Run the game once to create a config file. Inside the file you can set what items(s) you want to spawn. Separate items by semicolons. If you want to spawn a select number rather than for each player, add a hash symbol followed by the amount after each item. Example; ```Item Gun Handgun;Item Gun Shotgun#5;Item Gun Sledge Hammer``` will spawn a pistol and sledge hammer for each player with 5 sledge hammers.
