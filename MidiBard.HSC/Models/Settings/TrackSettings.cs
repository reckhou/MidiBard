using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC.Models.Settings
{
    [JsonObject(MemberSerialization.OptOut)]
    public class TrackSettings
    {

        public TrackSettings()
        {
            PopulateFromPlaylist = true;
        }

        public bool PopulateFromPlaylist { get; set; }

    }
}
