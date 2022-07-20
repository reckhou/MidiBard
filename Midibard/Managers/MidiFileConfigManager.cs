using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dalamud.Logging;
using Dalamud.Interface.Internal.Notifications;
using MidiBard.DalamudApi;
using Newtonsoft.Json;

namespace MidiBard.Managers
{
	static class MidiFileConfigManager
	{
		private static readonly JsonSerializerSettings JsonSerializerSettings = new()
		{
			//TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
			//TypeNameHandling = TypeNameHandling.Objects
		};

		public static FileInfo GetMidiConfigFileInfo(string songPath) => new FileInfo(Path.Combine(Path.GetDirectoryName(songPath), Path.GetFileNameWithoutExtension(songPath)) + ".json");

		public static MidiFileConfig? GetMidiConfigFromFile(string songPath)
		{
			var configFile = GetMidiConfigFileInfo(songPath);
			if (!configFile.Exists) return null;
			return JsonConvert.DeserializeObject<MidiFileConfig>(File.ReadAllText(configFile.FullName), JsonSerializerSettings);
		}

		public static void Save(this MidiFileConfig config, string path)
		{
			var fullName = GetMidiConfigFileInfo(path).FullName;
			File.WriteAllText(fullName, JsonConvert.SerializeObject(config, Formatting.Indented, JsonSerializerSettings));
		}

		public static MidiFileConfig GetMidiConfigFromTrack(IEnumerable<TrackInfo> trackInfos)
		{
			return new()
			{
				Tracks = trackInfos.Select(i => new DbTrack
				{
					Index = i.Index,
					Name = i.TrackName,
					Instrument = (int)(i.InstrumentIDFromTrackName ?? 0),
					Transpose = i.TransposeFromTrackName
				}).ToList(),
				AdaptNotes = MidiBard.config.AdaptNotesOOR,
				ToneMode = MidiBard.config.GuitarToneMode,
				Speed = 1,
			};
		}

		public static void Init()
		{
			LoadGlobalTrackMapping();
		}

		public static GlobalTrackMapping globalTrackMapping;

		static GlobalTrackMapping LoadGlobalTrackMapping()
		{
			var path = DalamudApi.api.PluginInterface.ConfigDirectory.FullName + $@"\MidiBardGlobalTrackMapping.json";
			FileInfo fileInfo = new FileInfo(path);
			if (!fileInfo.Exists)
            {
				PluginLog.LogWarning($"Global Track Mapping not exist, creating at {path}");
				SaveGlobalTrackMapping();
            }

			globalTrackMapping = JsonConvert.DeserializeObject<GlobalTrackMapping>(File.ReadAllText(path), JsonSerializerSettings);
			return globalTrackMapping;
		}

		static bool SaveGlobalTrackMapping()
        {
			if (globalTrackMapping == null)
            {
				globalTrackMapping = new GlobalTrackMapping();
            }

			var path = DalamudApi.api.PluginInterface.ConfigDirectory.FullName + $@"\MidiBardGlobalTrackMapping.json";
			try
			{
				var trackMappingFileInfo = GetGlobalTrackMappingFileInfo();
				if (trackMappingFileInfo != null)
				{
					var serializedContents = JsonConvert.SerializeObject(globalTrackMapping, Formatting.Indented);
					File.WriteAllText(trackMappingFileInfo.FullName, serializedContents);
					PluginLog.LogWarning($"{path} Saved");
				}
			} catch (Exception e)
            {
				PluginLog.LogError(e.ToString());
				return false;
            }

			return true;
		}

		static FileInfo GetGlobalTrackMappingFileInfo()
		{
			var pluginConfigDirectory = DalamudApi.api.PluginInterface.ConfigDirectory;
			return new FileInfo(pluginConfigDirectory.FullName + $@"\MidiBardGlobalTrackMapping.json");
		}

		public static void ExportToGlobalTrackMapping()
        {
			if (MidiBard.CurrentPlayback?.MidiFileConfig == null)
            {
				ImGuiUtil.AddNotification(NotificationType.Error, "Please choose a song first!");
				return;
            }

			var midiFileConfig = MidiBard.CurrentPlayback?.MidiFileConfig;
			Dictionary<long, List<int>> trackDict = new Dictionary<long, List<int>>();
			foreach(var cur in midiFileConfig.Tracks)
            {
				if (!trackDict.ContainsKey(cur.PlayerCid))
                {
					trackDict.Add(cur.PlayerCid, new List<int>());
                }

				trackDict[cur.PlayerCid].Add(cur.Index);
			}

			foreach(var pair in trackDict)
            {
				if (!globalTrackMapping.TrackMappingDict.ContainsKey(pair.Key))
                {
					globalTrackMapping.TrackMappingDict.Add(pair.Key, pair.Value);
				} else
                {
					globalTrackMapping.TrackMappingDict[pair.Key] = pair.Value;
                }
            }

			bool succeed = SaveGlobalTrackMapping();
			if (succeed)
			{
				ImGuiUtil.AddNotification(NotificationType.Success, "Global Track Mapping Exported.");
				IPC.IPCHandles.UpdateGlobalTrackMapping();
			} else
            {
				ImGuiUtil.AddNotification(NotificationType.Error, "Fail to Export Global Track Mapping!");
			}
		}
	}



	internal class MidiFileConfig
	{
		//public string FileName;
		//public string FilePath { get; set; }
		//public int Transpose { get; set; }
		public List<DbTrack> Tracks = new List<DbTrack>();
		//public DbChannel[] Channels = Enumerable.Repeat(new DbChannel(), 16).ToArray();
		//public List<int> TrackToDuplicate = new List<int>();
		public GuitarToneMode ToneMode = GuitarToneMode.Off;
		public bool AdaptNotes = true;
		public float Speed = 1;
	}

	internal class DbTrack
	{
		public int Index;
		public bool Enabled = true;
		public string Name;
		public int Transpose;
		public int Instrument;
		public long PlayerCid;
	}
	internal class DbChannel
	{
		public int Transpose;
		public int Instrument;
		public long PlayerCid;
	}

	internal class GlobalTrackMapping
    {
		public Dictionary<long, List<int>> TrackMappingDict = new Dictionary<long, List<int>>(); // PlayerCid - List of Track Indexes
    }
}
