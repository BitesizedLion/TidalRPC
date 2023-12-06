# TidalRPC

> [!NOTE]
> Versioning is `major.minor.revision.build`
> 
> It is not compatible with the "SteelSeries Engine" software.

![](https://user-images.githubusercontent.com/42980888/212440251-2c1ffe55-b132-4966-8327-88e86f46d8f5.png)
![](https://user-images.githubusercontent.com/42980888/212440257-ef827b46-06bf-44de-a165-024fa9b992b9.png)

TidalRPC is a program that uses Tidal's SteelSeries integration to display Discord Rich Presence (RPC), further explanation can be found further down.

The way it works is, it runs a web server on port 3650 (by default) which Tidal sends all track information to.

If you want to change the port, you need to edit `%programdata%\SteelSeries\SteelSeries Engine 3\coreProps.json`.

On first use, please right-click the tray icon and select your country (it should be a 2-character code)

### If you want to support the development of TidalRPC, you can donate at [https://www.buymeacoffee.com/caspr](https://www.buymeacoffee.com/caspr).

## Features

- Basic RPC Functionality
  - Track Info
  - Album Artwork (incl. Animated)
  - Toggle RPC option
  - Toggle Ads option (aka don't show presence when ads are playing)
- Auto-creates `coreProps.json`
- Clears presence when Tidal isn't running
- Update Checker (toggle is saved in Registry)
- Tray Icon


## Technical details

As mentioned earlier, the way this works is by using the integration Tidal's desktop app has with SteelSeries's [Engine software](https://steelseries.com/gg/engine). I'll refer to it as "Engine" from now on.

Engine launches a webserver on the IP:PORT specified in `%programdata%\SteelSeries\SteelSeries Engine 3\coreProps.json`.

Tidal finds this webserver thanks to the aforementioned file and POSTs the playback information to `/game_event` as JSON, this consists of:
- Song Title
- Artist
- Album
- Url (to the song)
- Image Url (to album cover)
- Duration (how much of the song has been played)
- Time (when the song started playing)
- State (indicates playback status, e.g. paused)
- 
