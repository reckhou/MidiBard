
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MidiBard.HSC.Models.Settings
{
    [JsonObject(MemberSerialization.OptOut)]
    public class AppSettings
    {

        public AppSettings()
        {
            this.GeneralSettings = new GeneralSettings();

            this.TrackSettings = new TrackSettings();
        }

        public GeneralSettings GeneralSettings { get; set; }

        public TrackSettings TrackSettings { get; set; }

        public string CurrentSong { get; set; }


        public int CurrentSongIndex { get; set; }
    }
}
