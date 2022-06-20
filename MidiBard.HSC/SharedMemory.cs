using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC
{
    public class SharedMemory
    {

        static MemoryMappedFile file;

        public static void CreateOrOpen()
        {
            file = MemoryMappedFile.CreateOrOpen("MidiBard.SharedMemory", 4);
        }

        public static void Write(byte[] buffer)
        {
            using (var stream = file.CreateViewStream())
            {
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        public static int  Read(byte[] buffer)
        {
            using (var stream = file.CreateViewStream())
            {
                int total = stream.Read(buffer, 0, buffer.Length);
                return total;
            }
        }
    }
}
