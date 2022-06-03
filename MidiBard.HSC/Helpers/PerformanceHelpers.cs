using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MidiBard.HSC.Enums;

namespace MidiBard.HSC.Helpers
{
    public class PerformanceHelpers
    {
        public static Instrument? GetInstrumentFromName(string insName)
        {
            if (string.IsNullOrEmpty(insName) || insName == "None")
                return Instrument.None;

            if (insName.Equals("Guitar (Overdriven)"))
                return Instrument.OverdrivenGuitar;

            if (insName.Equals("Guitar (Clean)"))
                return Instrument.CleanGuitar;

            if (insName.Equals("Guitar (Muted)"))
                return Instrument.MutedGuitar;

            if (insName.Equals("Guitar (Distorted)"))
                return Instrument.DistortedGuitar;

            return (Instrument)Enum.Parse(typeof(Instrument), insName);
        }
    }
}
