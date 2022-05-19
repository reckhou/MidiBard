
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Specialized;
using MidiBard.HSC;
using MidiBard.HSC.Helpers;

namespace MidiBard.HSC.Models.Playlist
{


    [JsonObject(MemberSerialization.OptIn)]
    public class Playlist
    {

        [JsonProperty]
        public string SettingsFile { get; set; }
        
        [JsonProperty]
        public string Title { get; set; }

        public List<PlaylistItem> Items { get; private set; }

        public int Total => this.Items.Count;

        public string FilePath { get; set; }

        public string FullPath => GetFullPath();

        public bool IsEmpty => this.Total == 0;

        public IEnumerable<string> Files => Items.IsNullOrEmpty() ? null : Items.Select(i => i.Sequence.Info.FilePath);


        public Playlist() 
        {
            this.Title = "playlist";
            this.FilePath = GetDefaultFilePath();
            this.Items = new List<PlaylistItem>();
        }
        
        public Playlist(string title) : this()
        {
            this.Title = title;
        }

        public Playlist(string title, IEnumerable<PlaylistItem> items) : this(title)
        {
            this.Items = new List<PlaylistItem>(items);
        }

        public void Remove(IEnumerable<string> files)
        {
            this.Items.RemoveAll(i => files.Contains(i.Sequence.Info.FilePath));
        }

        public void Remove(string file)
        {
            this.Items.RemoveAll(i => i.Equals(file));
        }

        public void Clear()
        {
            Items.Clear();
        }

        public static string GetDefaultFilePath()
        {
            return $"Playlists";
        }

        public PlaylistItem GetByIndex(int index)
        {
            return this.Items[index];
        }

        public PlaylistItem GetByPath(string path)
        {
            return this.Items.FirstOrDefault(i => i.Sequence.Info.FilePath == path);
        }

        public PlaylistItem GetByTitle(string title)
        {
            return this.Items.FirstOrDefault(i => i.Sequence.Info.Title == title);
        }


        private string GetFullPath()
        {
            return Path.Combine(this.FilePath, this.Title + ".pl");
        }

    }
}
