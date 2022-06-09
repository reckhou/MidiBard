using MidiBard.HSC.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC
{
    public static class Paths
    {
        public static string MidiFilePath { get; private set; }

        public static string PlaylistPath { get; private set; }

        static Paths()
        {
            MidiFilePath = Path.Combine(AppHelpers.GetAppAbsolutePath(), "Midis");

            PlaylistPath = Path.Combine(AppHelpers.GetAppAbsolutePath(), "Playlists");
        }
    }
}
