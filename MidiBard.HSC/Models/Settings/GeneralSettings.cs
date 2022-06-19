using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC.Models.Settings
{
    [JsonObject(MemberSerialization.OptOut)]
    public class GeneralSettings
    {

        public GeneralSettings()
        {
            EnableMidiBardTranspose = false;
            EnableMidiBardTrim = false;
            EnableMidiBardTrimFromTracks = false;
            EnableMidiBardInstrumentSwitching = false;
        }

        public bool EnableMidiBardTranspose { get; set; }

        public bool EnableMidiBardTrim { get; set; }

        public bool EnableMidiBardTrimFromTracks { get; set; }

        public bool EnableMidiBardInstrumentSwitching { get; set; }

        public bool CloseOnFinish { get; set; }

        public bool SendReadyCheckOnEquip { get; set; }

    }
}
