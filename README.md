# TidalRPC

**NOTICE: versioning is `major.minor.revision.build`**

![](https://user-images.githubusercontent.com/42980888/212440251-2c1ffe55-b132-4966-8327-88e86f46d8f5.png)
![](https://user-images.githubusercontent.com/42980888/212440257-ef827b46-06bf-44de-a165-024fa9b992b9.png)

TidalRPC is a program that uses Tidal's SteelSeries integration to display Discord Rich Presence (RPC).

It is not compatible with SteelSeries Engine software.

The way it works is, it runs a web server on port 3650 (by default) which Tidal sends all track information to.

To change the port, you need to go to `%programdata%\SteelSeries\SteelSeries Engine 3\coreProps.json`.

### If you want to support the development of TidalRPC, you can donate at [https://www.buymeacoffee.com/caspr](https://www.buymeacoffee.com/caspr).

## Features

- Basic RPC Functionality
  - Track Info
  - Album Artwork (incl. Animated)
  - Toggle RPC option
  - Toggle Ads option
- Auto-creates `coreProps.json`
- Clears presence when Tidal isn't running
- Update Checker (setting saved in Registry)
- Tray Icon
