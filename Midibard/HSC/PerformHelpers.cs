using Dalamud.Logging;
using MidiBard.HSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC
{
    public class PerformHelpers
    {
        public static uint GetInstrumentFromHscPlaylist(string fileName)
        {
            try
            {
                PluginLog.Information($"Instrument switching from hsc playlist for '{fileName}'");

                var songSettings = HSC.Settings.PlaylistSettings.Settings[fileName];

                var firstTrack = songSettings.Tracks.Values.FirstOrDefault(t => !t.Muted && t.EnsembleMember == HSC.Settings.CharIndex);
                if (firstTrack == null)
                    return 0;

                PluginLog.Information($"switching to '{firstTrack.EnsembleInstrument}' as assigned from hsc playlist");

                uint insId = (uint)PerformanceHelpers.GetInstrumentFromName(firstTrack.EnsembleInstrument).Value;

                return insId;
            }

            catch (Exception e)
            {
                PluginLog.Error(e, $"Instrument switching from hsc playlist failed. {e.Message}");
            }
            return 0;
        }
    }
}
