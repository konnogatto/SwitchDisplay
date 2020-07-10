# SwitchDisplay
A Playnite extension that switches primary display and audio output in a fullscreen mode


Download The extension using the link https://github.com/leonardoRC/SwitchDisplay/archive/v0.0.2.zip file then just drag and drop the file to Playnite's main window while it's running in Desktop mode. You will be prompted with installation dialog and after installation is complete you can change the screen selection at the Settings menu.

Fixing problems

update graphic card drivers Apply last windows updates Remember to restart Playnite after the configurations Any display change will be applied only on Playnite full-screen mode.

In the settings, both displays had the same name so I could not distinguish which was which. Yes, Generic UPnP Monitor, I seriously can't do anything about this, it depends on your drivers. There is not much information available in that API.

When I eventually got something to happen I ended up with it full screen on my PC monitor and regular Playnite on my TV. I could not get it to full screen on my TV no matter how I configured it. It relies on your second display to be enabled. Conversely, if you use WIN+P you can switch to your second display regardless of whether its enabled in display properties/Nvidia control panel. (I only ever switch between displays so its never otherwise enabled). This works as intended. It's not supposed to detect or enable/disable it automatically.

The takeaway thing from this is maybe it would work more reliably using the display switch command line?

DisplaySwitch.exe /internal DisplaySwitch.exe /external

