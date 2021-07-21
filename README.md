# Highlighter
Shadowplay alternative using OBS. Highlighter will automatically detect running games and configure OBS to record them using the Replay Buffer feature.
## Requirements
* Dotnet Core 3.1 Runtime Installed
* OBS v26.0 or higher
* Websocket plugin for OBS installed
## To Run
1. Build from source
1. Create game capture source in OBS, ie. "Game" 
1. Edit settings.json
    1. Set path to obs and recording directory
    1. Set sourceName to same name as in OBS, ie. "Game"
    1. Add games and executable names to "games" array
1. Bind replay buffer hotkey in OBS
1. Run Highlighter!