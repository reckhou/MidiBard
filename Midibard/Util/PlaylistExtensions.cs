using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.Util
{
    public static class PlaylistExtensions
    {
        public static int GetIndex(this List<string> pl, string title)
        {
            var match = pl.Select((n, i) => new { Name = n, Index = i }).FirstOrDefault(x => x.Name.ToLower() == title);
            return match == null ? -1 : match.Index;
        }
    }
}
