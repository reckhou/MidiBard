using Melanchall.DryWetMidi.Core;
using MidiBard.HSC.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC
{
    internal class MidiProcessor
    {
        public static void Process(MidiFile midiFile, string fileName)
        {

            if (!HSC.Settings.PlaylistSettings.Settings.ContainsKey(fileName))
                return;

            var settings = HSC.Settings.PlaylistSettings.Settings[fileName];

            if (Configuration.config.useHscmChordTrimming)
                ChordTrimmer.Trim(midiFile, settings, 2, false, Configuration.config.useHscmTrimByTrack);

            TrackUtilities.Shift(midiFile, settings);
        }

    }
}
