# **MidiBard 2**

**Please join our ✅ [Discord Server](https://discord.gg/xvNhquhnVT) for support!**

<p align="left">
  <a href="https://discord.gg/xvNhquhnVT">
    <img src="https://discord.com/api/guilds/897518233068920852/widget.png" alt="Discord">
  </a>
</p>

**If you appreciate our work, you can buy us some coffee following the link below:**

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/L3L6CQMMD)

`MidiBard 2` is an FF14 Bard plugin that is built on the [Dalamud](https://github.com/goatcorp/Dalamud) framework. `MidiBard 2` enables bard performance using MIDI files or keyboards, and features zero input latency and perfect sync between bards. The original author of this plugin is [akira0245](https://github.com/akira0245/MidiBard) and it is currently being developed by [Ori](https://github.com/reckhou/MidiBard2).

For more detailed information, please refer to the  [MidiBard Manuals](https://github.com/reckhou/MidiBard2/tree/v2-api7-stable/Manual).

If you need to get in touch with us, please feel free to email us at [midibard@proton.me](mailto:midibard@proton.me).



# What Makes MidiBard 2 Stand Out?

❖ High-quality playback that ensures a clean sound on every instrument with zero delay on playing notes. Even in crowded areas, the plugin never drops any notes, making it perfect for fast and busy songs.

❖ The plugin is based on in-game detection of bard ensemble mode, which offers almost perfect sync between bards.

❖ `MidiBard 2` automatically switches instruments by track names following BMP rules, and supports all songs from [Bard Music Player MIDI Repository](https://songs.bardmusicplayer.com/).

❖ The plugin is widely used by solo bards to octet bands and supports all file types used by Bard Music Player, LightAmp, and MogAmp. There is no need to set key bindings and hotbars for every bard.

❖ It switches songs and instruments across all bards in the same party through a local ensemble control panel or by commands. Bards don't have to be on the same PC, which means you can play with your friends. Everyone starts to play automatically by following the ensemble mode, and there's no need to countdown on Discord anymore!

❖ `MidiBard 2` plays any number of tracks on the MIDI file, transposes any track separately, or overrides the electric guitar's tone, which greatly helps for testing/performance. For example, composers may have 'Clean' and 'Overdriven' guitars on two tracks, both could be played by a single bard, making switching guitar tones much easier than editing MIDI files and adding events by hand.

❖ You are able to talk to your crowds when playing, making your show more lively. It also supports LRC files, which posts lyrics in the game in sync if you wish to sing along with your song. Additionally, the lyrics function supports all eight bards in your party, so you may appoint different singers freely.

❖ MidiBard 2 supports almost all MIDI keyboards and auto-adapts notes outside of C3-C6 to help test unadapted songs.

❖ Track visualization is also available, which helps with testing/debugging.

# How to Install
First you need to install and boot the game by using [FFXIVLauncher](https://github.com/goatcorp/FFXIVQuickLauncher).

You need to [Download the latest "Setup.exe" from the releases](https://github.com/goatcorp/FFXIVQuickLauncher/releases/latest)  page and run it. Once you do that, `XIVLauncher` will start and be installed in your start menu.

⚠  **Attention!**  If you encounter errors during installation or if the launcher does not function correctly, make sure to **check your antivirus** first and disable it for `XIVLauncher`. Many commercial antivirus programs may detect XIVLauncher as a false positive. If you need any help with this, please  [refer to the Dalamud FAQ](https://goatcorp.github.io/faq/xl_troubleshooting#q-how-do-i-whitelist-xivlauncher-and-dalamud-so-my-antivirus-leaves-them-alone).

After installing `XIVLauncher`, you will need to launch the game through it. Ensure that `Enable Dalamud` is enabled in the launcher settings.

![image](https://user-images.githubusercontent.com/1331053/224565608-b3c7b5d1-0499-42f3-bd23-a2910f5c73e3.png)

To access the Dalamud Plugins in-game, open the system menu by pressing `ESCAPE` and select `Dalamud Plugins` or `Dalamud Settings`. Alternatively, you can type `/xlplugins` command in chat.

![image](https://user-images.githubusercontent.com/1331053/224565755-884b157d-3902-4dc4-8637-aa1c8e9a849e.png)


To install `MidiBard 2`, please follow these steps to add our custom plugin repository:

1.  Open `Dalamud Settings`.
2.  Click on the `Experimental` tab.
3.  In the `Custom Plugin Repositories` section, enter the following URL: `https://raw.githubusercontent.com/reckhou/DalamudPlugins-Ori/api6/pluginmaster.json`
4.  Click on the `Save Changes and Close` icon, which is located in the bottom right corner of the window and looks like a hard disk.
5.  Return to the `Plugin Installer` and search for `Midibard`, install the plugin.

![image](https://user-images.githubusercontent.com/1331053/224566226-1d3a304c-e855-42ca-808b-5e3db469a2b4.png)

![image](https://user-images.githubusercontent.com/1331053/224566746-5aeafd36-d80f-4b33-a778-2feeebc7f56f.png)

Additionally, you may install `Bard Toolbox`, which is an all-in-one toolbox for all sorts of convenient features you may need. You may add it's URL in `Custom Plugin Repositories` as below:

`https://raw.githubusercontent.com/BardToolbox/BardToolbox-Release/master/pluginmaster.json`

Please [check Bard Toolbox's Github Page](https://github.com/BardToolbox/BardToolbox-Release) for more detailed descriptions.

# Recommend Settings

If you are new to `MidiBard 2`, please use those settings below. Those settings are recommended for the bands.

![image](https://user-images.githubusercontent.com/1331053/224567262-ef0ce0c1-63ae-41a0-8931-dfd68a591254.png)

# Quick Start

**For more detailed explanations, please check [MidiBard 2 Manual](https://raw.githubusercontent.com/reckhou/MidiBard2/v2-api6-stable/Manual/Midibard_Manual.pdf).**

* **Where to Start?**  

To open the `MidiBard` window, type `/midibard` in the chatbox. Once the window appears, click on the `+` icon to add MIDI files to the playlist. Hold down the `Ctrl` or `Shift` key to select multiple files. To switch to a specific song on the playlist, double-click it. Then, select the tracks you want to play and choose the instrument you wish to use.

![image](https://user-images.githubusercontent.com/1331053/224567609-d2ac5b1f-5860-421f-9c71-5e5be487f4b4.png)
![image](https://user-images.githubusercontent.com/1331053/224567643-61c2fb7c-3600-44e3-9e4c-40e910ac29e2.png)

Press the triangle icon to start playing.

If you are a solo bard, this is all you need to know 😊

* **How to Play as a Band**  

To play as a band, form a party with all of your bards. For first-time users, it's recommended to start with just two bards to get an idea of how it works. You can add more bards as you become more comfortable. Additionally, the bards do not have to be on the same PC, so you can play with friends in the same party.

* **If all of your bards are on the same device**

As the party leader, you can open the `Ensemble Panel` by clicking the corresponding button. From there, you can assign an instrument to each bard. The instruments will be automatically recognized based on the track names, [Track Name References For Auto-Switch Instruments are detailed in the appendix](https://github.com/reckhou/MidiBard2#bmp-track-name-references-for-auto-switch-instruments).

To switch instruments for each bard, click the guitar button on the `Ensemble Panel`. Once you've assigned the instruments, click the `Start Ensemble Mode` button on the top-left corner. This will automatically start the ensemble mode and all of your bards will play together.

![enter image description here](https://i.imgur.com/kOmttbG.png)
![enter image description here](https://i.imgur.com/TkYVzCj.png)

**To save time assigning your bards when switching to new songs, you can click the`Export to Default Performer` button if you consistently use the same track mappings, such as having the party leader always play track 1. This saves your track mappings and ensures that you don't have to assign your bards each time you switch to a new song.***

In addition, if you make changes to the instrument or track mappings, they will be saved to an additional config file and override the `Default Performer` setting on a per-song basis. However, new songs will still use the `Default Performer` setting.

* **Alternatively, if your bards are on different devices or if you want to play with friends:**

Enable the `Play on Multiple Devices` option in the settings. It is recommended to use file-sharing services like `Google Drive` to sync your songs and playlists across different devices. Make sure that the same drive volume is set on every device to maintain the same absolute file path, such as `J:\My Drive\Playlist`.

**You will need to choose tracks manually on every client, make sure everyone is using the same playlist.** Double click to switch to the desired song. **`MidiBard 2` will send commands to the party chat automatically to switch instruments. Once the instruments are selected, start the ensemble mode and all of your bards will begin playing together.

![image](https://user-images.githubusercontent.com/1331053/224568577-e5e69146-6972-4905-a347-1195930ac496.png)

**You only need to assign the tracks once after restarting the game. The bards will always play the same track number even if you switch to different songs.**


# Q&A

* **What is the best practice to automatically switch guitar tones?**

The easiest way to do this is by separating the tones onto different tracks. For example, you could have one track for clean guitar and another for overdriven guitar. Your bard should select both tracks. When playing the song, `MidiBard 2`will automatically switch between the guitar tones, so there's no need to manually edit the exported MIDI file to add hundreds of tone-switching events.

For example you may check: [MidiBard 2 - Multiple Guitar Tone Switching Showcase](https://www.youtube.com/watch?v=PfhYo1qKrSA)

* **Why does my song sound slower in some parts?**

This issue is typically caused by playing too many notes within a short period of time, which can exceed the game's limitations. While other software might randomly drop these notes, `MidiBard 2` is designed in a way not to do so, resulting in a delay as excess notes are queued and played later than intended. To address this, consider simplifying the song by reducing the number of notes played, particularly within chords, if possible.

* **Why Does My Performance Sound Laggy?**

Please check the following settings to ensure optimal performance:

![enter image description here](https://i.imgur.com/Sjvx8Df.png)
![enter image description here](https://i.imgur.com/nYNkUUO.png)


If your monitor has a higher refresh rate than 60Hz, it's recommended to limit it to 60Hz in the driver settings to prevent the client from consuming an excessive amount of resources. While it's possible to run the game at lower framerates, such as 15 FPS, `MidiBard 2` is designed to perform optimally at 60 FPS for a better experience.

*There are certain ways to disable the rendering of the game to save resources, but this is not the main focus of `MidiBard 2`. If you're interested in exploring this option, you may want to check out [Bard Toolbox](https://github.com/BardToolbox/BardToolbox-Release) for more details.*

* **I have further issues, where may I find support?**

If you encounter any issues with `MidiBard 2`, resetting the configuration can sometimes solve the problem. To access this option, right-click on `MidiBard 2` in `Dalamud Settings`.

![image](https://user-images.githubusercontent.com/1331053/224566807-291a06d1-ddb0-4d52-a0de-2c554513d6f9.png)

You can also back up your plugin configurations by navigating to the following folder:
`%AppData%\XIVLauncher\pluginConfigs`

Please join our ✅ [Discord Server](https://discord.gg/xvNhquhnVT) for support. Once you join the server, go to the [#role-assign](https://discord.com/channels/897518233068920852/897552592786296872) channel and react to the message shown below. Click the reaction for `MidiBard` so that you can be granted the `MidiBard User` role.

![image](https://user-images.githubusercontent.com/1331053/224570407-8e4e979d-8bdb-4194-8020-1c0100a2f64f.png)

You will then have access to [#midibard](https://discord.com/channels/897518233068920852/965700796769501295) channels as below:

![image](https://user-images.githubusercontent.com/1331053/224570519-906fbc0c-f38c-4700-8f98-85027e2ff43a.png)

# Party Chat Command References

Use these commands in the party chat to control all bards in the same party who have `MidiBard 2` installed. **These commands only work if the `Play on Multiple Devices` option is enabled.**


-   **switchto [song number]**

Switches to the Xth song on the playlist. For example, `switchto 2` will make every bard switch to the second song on the playlist (assuming everyone has the same playlist).

-   **close**

Stops playing and exits performance mode.

-   **reloadconfig**

Reloads the configuration file.

-   **speed [number]**

Sets the playback speed on all clients. 1 means normal speed, and the value should be larger than 0.1. For instance, `speed 2` makes the song play twice as fast.

-   **transpose [number]**

Sets the global transpose between all clients, excluding the drum tracks. For example, `transpose -2`.

-   **pmd [on|off] playonmultipledevices [on|off]**

Sets the option `Play on Multiple Devices` on all clients. For example, `pmd on` or `playonmultipledevices off`.

# Track Name References For Auto-Switch Instruments

Below are all the instruments supported in the game. We strongly recommend that the track names of the MIDI files follow these names, as `MidiBard 2` will automatically switch in-game instruments based on these names if the track is selected.

If you have many files and it becomes too difficult to rename all of the tracks, please let us know on our Discord server so that we can add some aliases to improve compatibility.

**The track names are NOT case-sensitive, and it doesn't matter if they have spaces or not. For example, `Double Bass` and `doublebass` are equivalent.**

| Instrument | Track Name |
|--|--|
| Piano | `piano` |
| Harp| `harp` |
| Fiddle| `fiddle` |
| Lute| `lute` |
| Fife| `fife` |
| Flute| `flute` |
| Oboe| `oboe` |
| Panpipes| `panpipes` |
| Clarinet| `clarinet` |
| Trumpet| `trumpet` |
| Saxophone| `saxophone`/`sax` |
| Trombone| `trombone` |
| Horn| `horn` |
| Tuba| `tuba` |
| Violin| `violin` |
| Viola| `viola` |
| Cello| `cello` |
| Double Bass| `doublebass`/`Double Bass`/`contrabass`|
| Bongo| `bongo` |
| Bass Drum| `bassdrum`/`Bass Drum` |
| Snare Drum|`snaredrum`/`Snare Drum`/`snare`  |
| Cymbal| `cymbal` |
| Timpani| `timpani` |
| Electric Guitar: Overdriven| `electricguitaroverdriven`/`Electric Guitar: Overdriven`/`programelectricguitar`/`program`/`electricguitar` |
| Electric Guitar: Clean| `electricguitarclean`/`Electric Guitar: Clean`|
| Electric Guitar: Muted| `electricguitarmuted`/`Electric Guitar: Muted` |
| Electric Guitar: Power Chords| `electricguitarpowerchords`/`Electric Guitar: Power Chords` |
| Electric Guitar: Special| `electricguitarspecial`/`Electric Guitar: Special` |

For transposition, you can add `+X` or `-X` after the instrument name to indicate a transposition. For example, `Trombone+1` indicates that the trombone track should be transposed up by one octave. This can help ensure that you have the correct range when editing and previewing your composition in `MuseScore`.

## Octave Ranges

|Instrument| Recommended Track Name| Range|
|--|--|--|
| Piano | Piano-1 |C4-C7|
| Harp| Harp |C3-C6|
| Fiddle| Fiddle+1|C2-C5|
| Lute| Lute+1|C2-C5|
| Fife| Fife-2|C5-C8|
| Flute| Flute-1|C4-C7|
| Oboe| Oboe-1|C4-C7|
| Panpipes| Panpipes-1|C4-C7|
| Clarinet| Clarinet|C3-C6|
| Trumpet| Trumpet|C3-C6|
| Saxophone| Saxophone|C3-C6|
| Trombone| Trombone+1|C2-C5|
| Horn| Horn+1|C2-C5|
| Tuba| Tuba+2|C1-C4|
| Violin| Violin|C3-C6|
| Viola| Viola|C3-C6|
| Cello| Cello+1|C2-C5|
| Double Bass| Double Bass+2|C1-C4|
| Timpani| Timpani+1|C2-C5|
| Electric Guitar: Overdriven| ElectricGuitarOverdriven+1|C2-C5|
| Electric Guitar: Clean| ElectricGuitarClean+1|C2-C5|
| Electric Guitar: Muted| ElectricGuitarMuted+1|C2-C5|
| Electric Guitar: Power Chords| ElectricGuitarPowerChords+1|C2-C5|
| Electric Guitar: Special| ElectricGuitarSpecial|C3-C6|
