using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MidiBard.HSC.Helpers
{
    public static class AppHelpers
    {

        public static string GetAppRelativePath(string path)
        {
            try
            {
                path = path.Substring(Settings.AppSettings.CurrentAppPath.Length + 1);
                return path;
            }
            catch
            {
                return path;
            }
        }
    }
}
