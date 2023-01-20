using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Dalamud;
using Dalamud.Configuration;
using Dalamud.Logging;
using Dalamud.Plugin;
using ImGuiNET;
using Newtonsoft.Json;

namespace MidiBard;

public class ConfigurationPrivate : IPluginConfiguration
{
    public static ConfigurationPrivate config;

    public int Version { get; set; }

    public bool[] EnabledTracks = Enumerable.Repeat(false, 100).ToArray();

    public static void Init()
    {
        Task.Run(() =>
        {
            bool loggedIn = false;
            while (!loggedIn)
            {
                try
                {
                    var CS = api.ClientState;
                    if (CS != null && CS.IsLoggedIn)
                    {
                        var playerData = CS.LocalPlayer;
                        var contentId = CS.LocalContentId;
                        if (playerData == null || playerData.HomeWorld.GameData == null)
                        {
                            Thread.Sleep(500);
                            continue;
                        }
                        
                        Load();
                        loggedIn = true;
                        return;
                    }
                }
                catch (Exception e)
                {
                    PluginLog.Error(e, "Error");
                    return;
                }

                Thread.Sleep(500);
            }
        });
    }

    public void Save()
    {
        try
        {
            var PI = api.PluginInterface;
            var CS = api.ClientState;
            if (CS != null && CS.IsLoggedIn)
            {
                var playerData = CS.LocalPlayer;
                var contentId = CS.LocalContentId;
                if (playerData != null && playerData.HomeWorld.GameData != null)
                {
                    var playerName = playerData.Name.TextValue;
                    var playerWorld = playerData.HomeWorld.GameData.Name.ToString();

                    var configFileInfo = GetConfigFileInfo(playerName, playerWorld, contentId);

                    var serializedContents = JsonConvert.SerializeObject(this, Formatting.Indented);

                    File.WriteAllText(configFileInfo.FullName, serializedContents);
                    PluginLog.LogWarning($"Saving {DateTime.Now} - {playerName}_{playerWorld}_{contentId}.json Saved");
                }
            }
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Error when saving private config");
            ImGuiUtil.AddNotification(Dalamud.Interface.Internal.Notifications.NotificationType.Error, "Error when saving private config");
        }
        
    }

    public static void Load()
    {
        var PI = api.PluginInterface;
        var CS = api.ClientState;
        if (CS != null && CS.IsLoggedIn)
        {
            var playerData = CS.LocalPlayer;
            var contentId = CS.LocalContentId;

            if (playerData != null && playerData.HomeWorld.GameData != null)
            {
                var playerName = playerData.Name.TextValue;
                var playerWorld = playerData.HomeWorld.GameData.Name.ToString();

                var configFileInfo = GetConfigFileInfo(playerName, playerWorld, contentId);
                if (configFileInfo.Exists)
                {
                    var fileText = File.ReadAllText(configFileInfo.FullName);

                    var loadedCharacterConfiguration = JsonConvert.DeserializeObject<ConfigurationPrivate>(fileText);
                    if (loadedCharacterConfiguration == null)
                    {
                        config = new ConfigurationPrivate();
                    } else {
                        config = loadedCharacterConfiguration;
                    }
                }
                else
                {
                    config = new ConfigurationPrivate();
                    config.EnabledTracks[0] = true; // always enable the 1st track for new user
                }
               return;
            }

            if (playerData == null)
            {
                PluginLog.LogDebug("PlayerData NULL");
            } else
            {
                PluginLog.LogDebug(playerData.HomeWorld.GameData == null ? "playerData.HomeWorld.GameData == null" : "");
            }
        }

        config = new ConfigurationPrivate(); // to prevent unexpected exception when character isn't logged in.
    }

    static FileInfo GetConfigFileInfo(string charName, string world, ulong contentID)
    {
        var pluginConfigDirectory = api.PluginInterface.ConfigDirectory;

        return new FileInfo(pluginConfigDirectory.FullName + $@"\{charName}_{world}_{contentID}.json");
    }
}