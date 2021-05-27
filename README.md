# skyrim-console-server

A proxy that intercepts [Dragonborn Speaks Naturally](https://github.com/YihaoPeng/DragonbornSpeaksNaturally)
to provide a weapon select interface and an HTTP API to send console commands
to Skyrim (unfortunately no output yet).  I created this because I was having
trouble getting VR quickslots to work.  I will split off DSN from this plugin
if I ever figure out how to code in C++ on Widnows, as well as add more
API methods.

## Usage

1. Install the ZIP using Vortex or NMM. Make sure you remove DSN if you have it
as this plugin contains it already. I do not advise installing manually, at
least for Skyrim SE, because some of the folders are copied from the VR version
to reduce the size of the download.
2. Start Skyrim and open the webpage in your browser. The default URL is
[http://localhost:12160/](http://localhost:12160/) but you can change it in
the INI.
3. Display the page in a window near your hand or other using either SteamVR beta
or OpenVR Toolkit.
4. Use OpenVR Toolkit chroma key to make the green background transparent, and
turn on angle and window hiding so the window disappears when not lifted close
to your face. Alternatively display it permanently at your waist.

## API Endpoints

* `POST /api/command`:<br />
Request Schema:
```json
{
    "type": "object",
    "properties": {
        "command": {
            "type": "string",
            "description": "The command to enter at the Skyrim CLI"
        }
    }
}
```
Response Schema:
```json
{
    "type": "object",
    "properties": {
        "status": {
            "type": "string",
            "description": "OK if the request was successful."
        }
    }
}
```
Example Request:
```json
POST /api/command
{
    "command": "player.equipitem ffffffff 0 right"
}
```
Example Response:
```json
200 OK
{
    "status": "OK"
}
```
* `GET /api/favorites`:<br />
Response Schema:
```json
{
    "type": "array",
    "items": {
        "type": "object",
        "properties": {
            "itemName": {
                "type": "string",
                "description": "The name of the item as interpreted by Dragonborn Speaks Naturally",
            },
            "formId": {
                "description": "The form ID, the main ID used in Creation Kit and other tools",
                "type": "number",
            },
            "itemId": {
                "description": "Probably the Object Reference ID?",
                "type": "number",
            },
            "isSingleHanded": {
                "description": "Can the item be wielded with one hand?",
                "type": "boolean",
            },
            "typeId": {
                "description": "The type number of the favorite. Weapons = 1, Spells = 2, etc.",
                "type": "number",
            }
        }
    }
}
```
Example Response:
```json
[
    {
        "itemName": "Iron Shield",
        "formId": 77494,
        "itemId": -468392994,
        "isSingleHanded": false,
        "typeId": 1
    },
    {
        "itemName": "Flames",
        "formId": 77773,
        "itemId": 0,
        "isSingleHanded": true,
        "typeId": 2
    },
]
```

## Tip for getting DSN to work better

Change your confidence value in the INI to around 0.2 if you can't get DSN to
recognize your speeech.

## Building

1. Build this solution.
2. Copy all files in the `bin/$(Configuration)` folder into a ZIP.
3. Install in Vortex or NMM
