using Dalamud.Logging;
using MidiBard.HSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiBard.Common;

namespace MidiBard.HSC
{
    public class PerformHelpers
    {
        public static uint GetInstrumentFromHscPlaylist(string fileName)
        {
            try
            {

                if (!HSC.Settings.PlaylistSettings.Settings.ContainsKey(fileName))
                    return 0;

                var songSettings = HSC.Settings.PlaylistSettings.Settings[fileName];

                if (songSettings.Tracks.IsNullOrEmpty())
                    return 0;

                var firstTrack = songSettings.Tracks.Values.FirstOrDefault(t => !t.Muted && t.EnsembleMember == HSC.Settings.CharIndex);
                if (firstTrack == null)
                    return 0;

                uint insId = (uint)PerformanceHelpers.GetInstrumentFromName(firstTrack.EnsembleInstrument).Value;

                return insId;
            }

            catch (Exception e)
            {
                PluginLog.Error(e, $"Instrument switching from hsc playlist failed. {e.Message}");
                return 0;
            }
        }
    }
}
