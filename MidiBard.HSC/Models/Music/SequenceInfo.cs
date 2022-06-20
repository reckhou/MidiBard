
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC.Music
{

    [Serializable]
    public class SequenceInfo
    {
        public SequenceInfo()
        {
        }

        public SequenceInfo(string filePath) : this()
        {
            FilePath = filePath;
            Title = Path.GetFileNameWithoutExtension(filePath);
        }

        public string FilePath { get; set; }

        public string Title { get; set; }

        public long FileSize { get; set; }

    }
}
