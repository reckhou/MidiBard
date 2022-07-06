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
    public class CharConfig
    {
        public const string CharConfigFileName = "characters.config";

        public static void Load()
        {
            string filePath = Path.Join(HSC.Settings.CurrentAppPath, CharConfigFileName);
            HSC.Settings.CharConfig = Common.FileHelpers.Load<CharacterConfig>(filePath);
        }

        public static void UpdateCharIndex(string charName)
        {
            Load();

            if (HSC.Settings.CharConfig == null || string.IsNullOrEmpty(charName))
                return;

            var chars = HSC.Settings.CharConfig.ToDictionary();

            HSC.Settings.CharIndex = chars.IsNullOrEmpty() || !chars.ContainsKey(charName) ? -1 : chars[charName];
        }
    }
}
