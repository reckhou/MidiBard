using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiBard.Common;

namespace MidiBard.HSC.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CharacterConfig
    {
        public CharacterConfig()
        {
            Characters = new List<string>();
        }

        public List<string> Characters { get; set; }

        public Dictionary<string, int> ToDictionary() =>
            Characters.IsNullOrEmpty() ? null :
            Characters.Select((c, i) => new { CharName = c, Index = i })
            .ToDictionary(c => c.CharName, c => c.Index);
    }
}
