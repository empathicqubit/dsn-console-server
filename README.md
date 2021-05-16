# EmpathicQbt.ConsoleServer

A proxy that intercepts [Dragonborn Speaks Naturally](https://github.com/YihaoPeng/DragonbornSpeaksNaturally)
to provide a weapon select interface and an HTTP API to send console commands
to Skyrim (unfortunately no output yet).  I created this because I was having
trouble getting VR quickslots to work.  I will split off DSN from this plugin
if I ever figure out how to code in C++ on Widnows, as well as add more
API methods.

## Usage

1. Install the ZIP using Vortex or NMM. Make sure you remove DSN if you have it
as this plugin contains it already.
2. Start Skyrim and open the webpage in your browser. The default URL is
[http://localhost:12160/](http://localhost:12160/) but you can change it in
the INI.
3. Display the page in a window near your hand or other using either SteamVR beta or OpenVR Toolkit.
4. Use OpenVR Toolkit chroma key to make the green background transparent, and
turn on angle and window hiding so the window disappears when not lifted close
to your face. Alternatively display it permanently at your waist.

## Tip for getting DSN to work better

Change your confidence value in the INI to around 0.2 if you can't get DSN to
recognize your speeech.

## Building

1. Download the [latest DSN ZIP](https://www.nexusmods.com/skyrimspecialedition/mods/16514)
to your Downloads folder.
2. Build this solution.
3. Copy all files in the `bin/$(Configuration)` folder into a ZIP.
4. Install in Vortex or NMM