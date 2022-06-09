using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC
{
    public class Enums
    {
        public enum RepeatType
        {
            LoopNone, LoopOne, LoopAll, Shuffle
        }

		public enum Instrument
		{
			None = 0,
			Harp = 1,
			Piano = 2,
			Lute = 3,
			Fiddle = 4,
			Flute = 5,
			Oboe = 6,
			Clarinet = 7,
			Fife = 8,
			Panpipes = 9,
			Timpani = 10,
			Bongo = 11,
			BassDrum = 12,
			SnareDrum = 13,
			Cymbal = 14,
			Trumpet = 15,
			Trombone = 16,
			Tuba = 17,
			Horn = 18,
			Saxophone = 19,
			Violin = 20,
			Viola = 21,
			Cello = 22,
			DoubleBass = 23,
			ElectricGuitarOverdriven = 24,
			ElectricGuitarClean = 25,
			ElectricGuitarMuted = 26,
			ElectricGuitarPowerChords = 27,
			ElectricGuitarSpecial = 28,
		}
	}
}
