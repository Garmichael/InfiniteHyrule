# Infinite Hyrule

**Infinite Hyrule**

This randomizer procedurally generates a whole new overworld map for the original Legend of Zelda for the NES.

My other projects can be found at [My Studio Site](https://www.stormgardenstudio.com/)

If you'd like to support the development of this, and other projects I work on, consider supporting me through [My Venmo](https://www.venmo.com/Garmichael).

---

### [Downloads](https://github.com/Garmichael/InfiniteHyrule/releases)

### [Join the Discord](https://discord.gg/F4tpUHJsvj)

# Screenshots

![Screenshot](/Screenshots/applicationSS.png?raw=true&ver=1)
![Screenshot](/Screenshots/overworldss1.png?raw=true&ver=1)
![Screenshot](/Screenshots/overworldss2.png?raw=true&ver=1)
![Screenshot](/Screenshots/overworldss3.png?raw=true&ver=1)
![Screenshot](/Screenshots/overworldss4.png?raw=true&ver=1)


![Screenshot](/Screenshots/ss1.png) ![Screenshot](/Screenshots/ss2.png) ![Screenshot](/Screenshots/ss3.png) ![Screenshot](/Screenshots/ss4.png) ![Screenshot](/Screenshots/ss5.png) ![Screenshot](/Screenshots/ss6.png) ![Screenshot](/Screenshots/ss7.png) ![Screenshot](/Screenshots/ss8.png) ![Screenshot](/Screenshots/ss9.png) ![Screenshot](/Screenshots/ss10.png) ![Screenshot](/Screenshots/ss11.png) ![Screenshot](/Screenshots/ss12.png) ![Screenshot](/Screenshots/ss13.png) ![Screenshot](/Screenshots/ss14.png) ![Screenshot](/Screenshots/ss15.png) ![Screenshot](/Screenshots/ss16.png) ![Screenshot](/Screenshots/ss17.png) ![Screenshot](/Screenshots/ss18.png) ![Screenshot](/Screenshots/ss19.png) ![Screenshot](/Screenshots/ss20.png) ![Screenshot](/Screenshots/ss21.png) ![Screenshot](/Screenshots/ss22.png) ![Screenshot](/Screenshots/ss23.png) ![Screenshot](/Screenshots/ss24.png) ![Screenshot](/Screenshots/ss25.png) ![Screenshot](/Screenshots/ss26.png) ![Screenshot](/Screenshots/ss27.png) ![Screenshot](/Screenshots/ss28.png) ![Screenshot](/Screenshots/ss29.png) ![Screenshot](/Screenshots/ss30.png) ![Screenshot](/Screenshots/ss31.png) ![Screenshot](/Screenshots/ss32.png) ![Screenshot](/Screenshots/ss33.png) ![Screenshot](/Screenshots/ss34.png) ![Screenshot](/Screenshots/ss35.png) ![Screenshot](/Screenshots/ss36.png) ![Screenshot](/Screenshots/ss37.png) ![Screenshot](/Screenshots/ss38.png)

## Credits

Garret Bright - Development and Production of Infinite Hyrule

**Special Thanks**

[Cyneprepou4uk](https://www.romhacking.net/forum/index.php?action=profile;u=75353) - Wrote an ASM patch for the original rom to expand the number of banks and give each screen its own unique layout, circumventing limitations for screen designs.

## License


Infinite Hyrule is a project licensed under the terms of the GPLv3, which means that you are given legal permission to copy, distribute and/or modify this project, as long as:

1.  The source for the available modified project is shared and also available to the public without exception.
2.  The modified project subjects itself different naming convention, to differentiate it from the main and licensed Infinite Hyrule.

You can find a copy of the license in the [LICENSE](https://bitbucket.org/Garmichael/infinite-hyrule/src/master/LICENSE) file.



# Change Log

**V 2.21**

- Updated Z1R Compatibility Guide.
- Fixed a bug where Generating a new map would also generate a new seed.

**v 2.20**

- Options to toggle the extra biomes (The River, The Ghost Forest, and the Kakariko Village).
- Options to toggle the enhanced versions of the Desert and the Beaches.
- Options to hide specific dungeons, including hiding all dungeons behind Bomb Walls and Burnable Bushes.
- Flagset hash for quickly setting options.
- Flagset Presets available from a drop-down.
- Kakariko can now have secret caves (Will replace a door with a bomb wall).
- Updated instructions on using Infinite Hyrule in conjunction with Z1R.
- Added instructions for using the Automapper Lua Scripts.
- Updated the rom output filename structure.
- Less likely to fail generating a map from any given seed.
- Slighty faster generation.
- Fixed some random crashes.
- Fixed a very rare bug where the Overworld Ladder Item was inaccessible on certain screens.

**v 2.14**

- Added a dialog box describing which Roms are compatible
- Fixed a bug that would cause blue pixel corruption on some Sand Tiles
- Fixed a bug that would crash the game when exiting an Armos Cave on a screen with too few ground tiles

**v 2.11**

- Moved the button that opened the Z1R Compatibility guide to the screen before the Rom is loaded
- Fixed a bug where sometimes Armos formations wouldn't be generated on islands

**v 2.10**

- Added a Z1R compatibility flag to use Overworld Quest 1
- Fixed a bug that would sometimes cause a weird opening along the map's edges
- Fixed a bug where the secret Armos Item would appear on the same screen as the Overworld Heart Container

**v 2.00**

- Added a new Biome for Kakario Village
- An optional setting to make Bombable Walls and Burnable Bushes visually distinct
- There are now a few single-screen lakes with Secrets underneath, one of which is now the location to Dungeon 7
- The Secret Screen has been added to the overworld Generation that can only be accessed by finding a false wall on the screen below it. Originally, this was the top right corner of the map
- An optional setting to hide Dungeon 8 on the Secret Screen and Dungeon 9 under any Burnable Bush or Bombable Wall
- When choosing the setting to hide Dungeon 9, a new cave will be placed in the overworld with a free hint guiding the player to the biome where Dungeon 9 can be found.
- Death Mountain now takes up fewer screens and will be placed farther away from Link's starting screen.
- Open and Hidden Caves now appear in Graveyards more often
- Palm Trees can now be found along the beach and at the Oasis
- Bridges that travel North and South are now rotated
- When Generating a map with a preview, there is now a Spoiler Block the user has to click to reveal the map in Infinite Hyrule. Saving the rom without revealing the map in the application will still output a PNG of the map to the same folder as the Rom.
- Enemy Counts have been tweaked to reduce the chances of a screen being too laggy
- A new Popup Window explaining how to use Infinite Hyrule with Zelda 1 Randomizer by Fred Coughlin
- And a bunch of boring edge-case bug fixes.. like screens or bridges being blocked sometimes, staircases sometimes exiting Link in the wrong spot, rafts not working, that sort of thing.
- Some Generation Optimization. Maps should generate much faster than before
