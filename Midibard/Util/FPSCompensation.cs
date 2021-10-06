using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using MidiBard.DalamudApi;
using System.Runtime.InteropServices;
using Dalamud.Logging;
using MidiBard.Control.MidiControl;

namespace MidiBard.Util
{
	public static class FPSCompensation
	{
		public static int RefFPS = 60;
		public static int DetectAvgFPSInSeconds = 5;
		private static List<float> fpsHistory;
		private static Stopwatch fpsHistoryInterval;
		public static float RefFrameLength = 1 / RefFPS;
		public static float AvgFrameLength, Compensation, OldCompensation;
		public static float magicNumber = 2;

		public static void Start()
		{
			fpsHistoryInterval = new Stopwatch();
			fpsHistoryInterval.Start();
			fpsHistory = new List<float>();
		}

		public static void Tick()
		{
			if (fpsHistoryInterval == null)
			{
				Start();
				return;
			}

			if (fpsHistoryInterval.ElapsedMilliseconds > 1000)
			{
				fpsHistoryInterval.Restart();
				// FPS values are only updated in memory once per second.
				var fps = Marshal.PtrToStructure<float>(api.Framework.Address.BaseAddress + 0x165C);
				
				if (fpsHistory.Count >= DetectAvgFPSInSeconds)
				{
					fpsHistory.RemoveAt(0);
				}

				fpsHistory.Add(fps);

				float sum = 0;
				foreach(var cur in fpsHistory)
				{
					sum += cur;
				}
				float avgFPS = sum / fpsHistory.Count;

				AvgFrameLength = avgFPS > RefFPS ? 1 / RefFPS : 1 / avgFPS;
				OldCompensation = Compensation;
				magicNumber = avgFPS > RefFPS ? 1 : MathF.Pow(RefFPS / avgFPS, 2.0f);
				Compensation = (AvgFrameLength - RefFrameLength) / magicNumber;
				//PluginLog.LogWarning("Mnumber:" + magicNumber + "FPS: " + fps + " avg: " + avgFPS + " compensation: " + Compensation);
				FilePlayback.ChangeCompensation(Compensation);
			}
		}
	}
}