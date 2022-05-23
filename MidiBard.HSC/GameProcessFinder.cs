using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiBard.Common;

namespace MidiBard.HSC
{
    public class GameProcessFinder
    {
        public static Process[] Find()
        {
            Process[] processes = Process.GetProcessesByName("ffxiv_dx11");

            if (processes.IsNullOrEmpty())
                return null;

            return processes.Where(p => p.MainWindowHandle.ToInt32() > 0 && !p.HasExited && !p.MainWindowTitle.IsNullOrEmpty()).ToArray();
        }

        public static int GetIndex(int processId)
        {
            var processes = Find();

            var process = processes.Select((p, i) => new { Index = i, Process = p })
                .FirstOrDefault(p => p.Process.Id == processId);

            return process == null ? -1 : process.Index;
        }

    }
}
