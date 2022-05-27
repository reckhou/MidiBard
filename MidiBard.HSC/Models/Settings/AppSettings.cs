
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MidiBard.HSC.Models.Settings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AppSettings
    {

        public AppSettings()
        {

        }


        [JsonProperty]
        public string PrevPlaylistFileName { get; set; }

        [JsonProperty]
        public string PrevPlaylistPath { get; set; }

        [JsonProperty]
        public string PrevSequenceTitle { get; set; }

        [JsonProperty]
        public string PrevMidiPath { get; set; }

        public string CurrentAppPath { get; set; }

        public bool MidibardPlaylistChanged { get; set; }

        public bool MidibardPlaylistSettingsChanged { get; set; }
    }
}
