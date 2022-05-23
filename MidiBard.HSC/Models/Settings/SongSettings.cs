using MidiBard.HSC.Music;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiBard.Common;

namespace MidiBard.HSC.Models.Settings
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class SongSettings
    {
        public Dictionary<string, MidiSequence> Settings;

        public SongSettings()
        {
            Settings = new Dictionary<string, MidiSequence>();
        }

        public void Clear()
        {
            Settings.Clear();
        }


        public void Remove(IEnumerable<string> files)
        {
            foreach(var f in files)
                this.Settings.Remove(f);
        }

        public void Remove(string file)
        {
            this.Settings.Remove(file);
        }

        public bool HasItems => !Settings.IsNullOrEmpty();

        public void Dispose()
        {
            Clear();
            Settings = null;
        }
    }
}
