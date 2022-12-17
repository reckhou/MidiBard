

# **MidiBard 2**

**Please join our ✅ [Discord Server](https://discord.gg/xvNhquhnVT) for support!**

<p align="left">
  <a href="https://discord.gg/xvNhquhnVT">
    <img src="https://discord.com/api/guilds/897518233068920852/widget.png" alt="Discord">
  </a>
</p>

**Buy us some coffee if you wish:**

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/L3L6CQMMD)

`MidiBard 2` is an FF14 Bard plugin based on the [Dalamud](https://github.com/goatcorp/Dalamud) framework, which supports bard performance by MIDI files or keyboards. It is originally authored by [akira0245](https://github.com/akira0245/MidiBard) and developing by [Ori](https://github.com/reckhou/MidiBard2).

For more detailed information, please check [MidiBard Manuals](https://github.com/reckhou/MidiBard2/tree/v2-api7-stable/Manual).

For contact you may use our E-mail: [midibard@proton.me](mailto:midibard@proton.me).



# Why MidiBard 2?

❖ High-quality playback, clean sound on every instrument. Minimal delay on playing notes, never drop any notes in crowded areas. Suits especially well for fast and busy songs.

❖ Based on in-game detection of bard ensemble mode, almost perfect sync between bards. Also supports manual sync if you wish to add a little bit of flavour to your song.

❖ Automatically switches instruments by track names following BMP rules, all songs from [BMP MIDI Repository](https://bmp.trotlinebeercan.com/) are supported.

❖ No need to set key bindings and hotbars on your every bard.

❖ Switches songs and instruments across all bards in the same party, through local ensemble control panel or by commands. Those bards don't have to be on the same PC, which means it's possible to play with your friends, and everyone starts to play automatically by following the ensemble mode, no need to countdown on Discord anymore!

❖ Plays any number of tracks on the MIDI file, transposes any track separately, or overrides the electric guitar's tone, which greatly helps for testing/performance. 

For example, composers may have `Clean` and `Overdriven` guitars on two tracks, both could be played by a single bard, which makes switching guitar tones much easier than editing MIDI files by hand.

❖ Able to talk to your crowds when playing, makes your show more lively!

❖ Supports LRC file, posts lyrics in the game in sync, if you wish to sing along with your song.

❖ Supports almost all MIDI keyboards.

❖ Auto-adapt notes outside of C3-C6 to help test unadapted songs.

❖ Track visualization, helps for testing/debugging.

# How to Install
You need to install and boot the game by using [FFXIVLauncher](https://github.com/goatcorp/FFXIVQuickLauncher).

This tutorial assumes you installed `FFXIVLauncher` and boot the game by it, and you have the basic knowledge of the usage of the `Dalamud plugin system`.

You need to add our custom plugin repository to install MidiBard:  
`https://raw.githubusercontent.com/reckhou/DalamudPlugins-Ori/api6/pluginmaster.json` 

Please open ``Dalamud Settings``, on ``Experimental`` page, add a ``Custom Plugin Repository`` as below:

![DD5JTJH.png](https://i.imgur.com/ryHkqnU.png)

Back to `Plugin Installer`, search `Midibard 2` and install.

![enter image description here](https://i.imgur.com/MoKudpz.png)

# Recommend Settings

If you are new to `MidiBard 2`, please use those settings below. Those settings are recommended for the bands.

![image](https://i.imgur.com/yEtHOM5.png)

# How to Use

**For more detailed explanations, please check [MidiBard 2 Manual](https://raw.githubusercontent.com/reckhou/MidiBard2/v2-api6-stable/Manual/Midibard_Manual.pdf).**

* **Where to Start?**  

Type `/midibard` in the chatbox and the window will show up. Press the `+` icon to add MIDI files to the playlist(hold `Ctrl` or `Shift` key to choose multiple files).  Double click any songs on the playlist to switch. Select the tracks you wish to play, and choose the instrument you wish to use. 

![enter image description here](https://i.imgur.com/z0vkgrh.png)

Press the triangle icon to start playing.

If you are a solo bard, this is all you need to know 😊

* **How to Play as a Band**  

Form a party with all of your bards. For the first time user, I recommend trying it on 2 bards to help you get the idea of how it works, the rest are just adding more bards. Also the bards **DON'T** have to be on the same PC, which means it's possible to play with your friends at the same party.

* **If all of your bards are on the same device**

On party leader, click the `Ensemble Panel` button to open ensemble panel. Assign your bards and instruments from there. Click the guitar button(optional) to switch instruments, then start the ensemble mode. All of your bards will start to play automatically.

![enter image description here](https://i.imgur.com/kOmttbG.png)
![enter image description here](https://i.imgur.com/TkYVzCj.png)
![enter image description here](https://i.imgur.com/sh1wIpC.png)

**You may also `Export to Default Performer`, then you don't need to assign your bards again when switching to new songs.**

* **Alternatively, if your bards are on different devices, or to play with your friends**

Tick the `Play on Multiple Devices` option. Choose tracks from every client, then switch to the song you want to play. `MidiBard` will send commands to the party chat automatically to switch instruments. Start the ensemble mode then all of your bards will start to play automatically.

![enter image description here](https://i.imgur.com/Fz5qsJ0.png)

**You only need to assign the tracks once after rebooting the game. They will always play the same track number even on different songs.**


# Q&A

* **How to Automatically Switch Guitar Tones?**

The easiest way is to separate tones on different tracks. For instance, you may have one track for clean guitar and another track for overdriven guitar. Your bard should choose both tracks and check if tones are being set correctly. When playing the song MidiBard switches the guitar tone automatically, and there's no need to edit the exported MIDI file to add tone-switching events.

* **Why My Song Sounds "Slower" at Some Parts?**

It's often caused by too many notes being played in a very short period, and it may exceed the game's limitation already. Other software might drop these notes, but MidiBard is designed not to drop any notes. So you may imagine that excessive amount of notes being put in a queue and they are being played later than they should be. You might want to check if your song is too busy, especially for chords and remove some notes if it's possible.

* **Why My Performance Sounds Lagging?**  
Please follow those settings shown below:

![enter image description here](https://i.imgur.com/Sjvx8Df.png)
![enter image description here](https://i.imgur.com/nYNkUUO.png)

If your monitor has a higher refresh rate than 60Hz, Please limit it to 60 so the client doesn't take an excessive amount of resources.

We have tested to run the game under 15 FPS, and MidiBard still sounds okay and never drops notes under lower framerates. However, it's not recommended and you should always aim for 60 FPS for a better experience.

*There are also certain ways to disable the rendering of the game to make it run more smoothly, however, it's not what MidiBard is trying to do and we will not support any requests suggesting we do so.*

* **Where May I Find Support?**

Please join our ✅ [Discord Server](https://discord.gg/xvNhquhnVT) for support. After joined the server, head to `role-assign` channel and react to this message show below. Please click the MidiBard so you will be granted `MidiBard User` role:

![enter image description here](https://i.imgur.com/lccLSDv.png)
![enter image description here](https://i.imgur.com/2vMGfnP.png)

# Party Chat Command References

Type these commands in the party chat to control all bards in the same party that has MidiBard installed.

* **switchto [song number]**

Switches to the Xth song on the playlist. e.g. `switchto 2` will make every bard switch to the 2nd song on the playlist(assuming everyone has the same playlist).

* **close**

Stops playing and quit performance mode.

*  **reloadconfig**

Reloads the config file.

*  **speed [number]**

Sets playback speed on all clients. 1 means normal speed, the value should be larger than 0.1. e.g. `speed 2` makes the song play 2x faster.

*  **transpose [number]**

Sets global transpose between all clients, the drum tracks won't be affected. e.g. `transpose -2`.

*  **pmd [on|off] playonmultipledevices [on|off]**

Sets the option `Play on Multiple Devices` on all clients. e.g `pmd on` or `playonmultipledevices off`.

# BMP Track Name References For Auto-Switch Instruments

Below are all instruments supported in the game. The track name of the MIDI file must follow those names, and MidiBard will switch in-game instruments by those names if the track is selected.

>Piano
>Harp
>Fiddle
>Lute
>Fife
>Flute
>Oboe
>Panpipes
>Clarinet
>Trumpet
>Saxophone
>Trombone
>Horn
>Tuba
>Violin
>Viola
>Cello
>Double Bass
>Timpani
>ElectricGuitarClean 
>ElectricGuitarMuted 
>ElectricGuitarOverdriven
>ElectricGuitarPowerChords 
>ElectricGuitarSpecial

For transposition, add `+X` or `-X` after the instrument name. For instance, `Trombone+1` means +1 octave on the trombone track. This is especially helpful in composing software like `MuseScore`, so you can have the correct range when editing.

**Octave Ranges**

>Piano-1: C4-C7 
>Harp: C3-C6 
>Fiddle+1: C2-C5 
>Lute+1: C2-C5
>Fife-2 C5-C8 
>Flute-1: C4-C7 
>Oboe-1: C4-C7 
>Panpipes-1: C4-C7 
>Clarinet: C3-C6
>Trumpet: C3-C6 
>Saxophone: C3-C6
>Trombone+1: C2-C5
>Horn+1: C2-C5
>Tuba+1: C1-C4
>Violin: C3-C6
>Viola: C3-C6
>Cello+1: C2-C5
>Double Bass+2: C1-C4
>Timpani+1: C2-C5
>All Guitars+1: C2-C5
