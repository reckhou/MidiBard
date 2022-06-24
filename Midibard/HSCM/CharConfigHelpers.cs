using MidiBard.HSC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiBard.Common;
using MidiBard.HSC;
using Dalamud.Logging;

namespace MidiBard
{
    public class CharConfigHelpers
    {
        public const string CharConfigFileName = "characters.config";

        public static int GetCharIndex(string charName)
        {
            string filePath = Path.Join(HSC.Settings.CurrentAppPath, CharConfigFileName);
            var charConfig = Common.FileHelpers.Load<CharacterConfig>(filePath);

            if (charConfig == null || string.IsNullOrEmpty(charName))
                return -1;

            var chars = charConfig.ToDictionary();

            return chars.IsNullOrEmpty() || !chars.ContainsKey(charName) ? -1 : chars[charName];
        }
    }
}
