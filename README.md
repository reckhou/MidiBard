

# **MidiBard 2**

**Please join our ✅ [Discord Server](https://discord.gg/xvNhquhnVT) for support!**

**Buy me a coffee if you wish:**

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/P5P6DAAKQ)

`MidiBard 2` is an FF14 Bard plugin based on the [Dalamud](https://github.com/goatcorp/Dalamud) framework, which supports bard performance by MIDI files or keyboards. It is originally authored by [akira0245](https://github.com/akira0245/MidiBard), developing by [Ori](https://github.com/reckhou/MidiBard) and [SpuriousSnail86](https://github.com/SpuriousSnail86/MidiBard).


# Why MidiBard 2?

❖ High-quality playback, clean sound on every instrument. Minimal delay on playing notes, never drop any notes in crowded areas. Suits especially well for fast and busy songs.

❖ Based on in-game detection of bard ensemble mode, almost perfect sync between bards. Also supports manual sync if you wish to add a little bit of flavour to your song.

❖ Automatically switches instruments by track names following BMP rules, all songs from [BMP MIDI Repository](https://bmp.trotlinebeercan.com/) are supported.

❖ No need to set key bindings and hotbars on your every bard.

❖ Switches songs and instruments across all bards in the same party, by commands. Those bards don't have to be on the same PC, which means it's possible to play with your friends, and everyone starts to play automatically by following the ensemble mode, no need to countdown on Discord anymore!

❖ Plays any number of tracks on the MIDI file, transposes any track separately, or overrides the electric guitar's tone, which greatly helps for testing/performance. 

For example, composers may have `Clean` and `Overdriven` guitars on two tracks, both could be played by a single bard, which makes switching guitar tones much easier than editing MIDI files by hand.

❖ Able to talk to your crowds when playing, makes your show more lively!

❖ Supports LRC file, posts lyrics in the game in sync, if you wish to sing along with your song.

❖ Supports almost all MIDI keyboards.

❖ Auto-adapt notes outside of C3-C6 to help test unadapted songs.

❖ Track visualization, helps for testing/debugging.

# How to Install
To use you need to install and boot the game by using [FFXIVLauncher](https://github.com/goatcorp/FFXIVQuickLauncher).

This tutorial assumes you installed FFXIVLauncher and boot the game by it, and you have the basic knowledge of the usage of the Dalamud plugin system.

You need to add my custom plugin repository to install MidiBard.  
`https://raw.githubusercontent.com/reckhou/DalamudPlugins-Ori/api6/pluginmaster.json` 

Please open ``Dalamud Settings``, on ``Experimental`` page, add a ``Custom Plugin Repository`` as below:

![DD5JTJH.png](https://i.imgur.com/ryHkqnU.png)

Back to `Plugin Installer`, search `Midibard 2` and install.

![enter image description here](https://i.imgur.com/LB6UMEk.png)

# Recommend Settings

If you are new to `MidiBard 2`, please use those settings below. Those settings are recommended for the band.

![image](https://i.imgur.com/mGXTy0O.png)

# How to Use

**For more detailed explanations, please check [MidiBard 2 Manual](https://raw.githubusercontent.com/reckhou/MidiBard2/v2-api6-stable/Manual/Midibard_Manual.pdf).**

* **Where to Start?**  

Type `/midibard` in the chatbox and the window will show up. Press the `+` icon to add MIDI files to the playlist(hold `Ctrl` or `Shift` key to choose multiple files).  Double click any songs on the playlist to switch. Select the tracks you wish to play, and choose the instrument you wish to use. 

![enter image description here](https://i.imgur.com/Iu9wVyQ.png)

Press the triangle icon to start playing.

If you are a solo bard, this is all you need to know 😊

* **How to Play as a Band**  

1. Form a party with all of your bards. For the first time user, I recommend trying it on 2 bards to help you get the idea of how it works, the rest are just adding more bards. Also the bards **DON'T** have to be on the same PC, which means it's possible to play with your friends at the same party.
2. After forming the party, import the MIDI files then immediately type `reloadplaylist` on **PARTY CHAT**. All bards on the **SAME PC** will then share the same playlist. You only need to do this once after changing the playlist. If you play with your friends, make sure everyone has the same playlist before continuing.
3. Type `switchto <song number>` in the party chat, then every bard in the party will load the same song and switch instruments if the track name follows BMP standards. For example, typing "switchto 1" will make everyone in the party open the 1st song on the playlist.
4. Check every bard to make sure everyone's choosing the correct track they are playing. You only need to do it once after rebooting the game or reloading the playlist. They will always play the same track number even on different songs.
5. After everything's been set, start the ensemble mode in-game and the song will start playing automatically.

![enter image description here](https://i.imgur.com/ZxsXKKp.png)



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

Switching to the Xth song on the playlist. e.g. `switchto 2` will make every bard switch to the 2nd song on the playlist(assuming everyone has the same playlist).
* **close**

Stop playing and quit performance mode.

*  **reloadplaylist**

Reload the config file which saves the playlist. This can be used after any changes to the playlist so the clients on the same PC can have the same playlist.

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
