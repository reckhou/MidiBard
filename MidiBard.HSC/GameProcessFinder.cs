using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiBard.Common;

namespace MidiBard.HSC
{
    public class ProcessFinder
    {
        public static Process[] Find(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);

            if (processes.IsNullOrEmpty())
                return null;

            return processes.Where(p => p.MainWindowHandle.ToInt32() > 0 && !p.HasExited && !p.MainWindowTitle.IsNullOrEmpty()).ToArray();
        }

    }
}
