using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.Common.IPC
{
    public class SharedMemory
    {

        static MemoryMappedFile mmf;
        static MemoryMappedViewAccessor mmva;

        public static bool CreateOrOpen()
        {
            mmf = MemoryMappedFile.CreateOrOpen("MidiBard.SharedMemory", 64);
            if (mmf == null)
                return false;
            mmva = mmf.CreateViewAccessor(0, 64, MemoryMappedFileAccess.ReadWrite);
            if (mmva == null)
                return false;
            return true;
        }

        public static void Write(int[] buffer)
        {
            mmva?.WriteArray<int>(0, buffer, 0, buffer.Length);
        }

        public static int Read(int[] buffer, int size)
        {
            if (mmva == null)
                return 0;

            int total = mmva.ReadArray<int>(0, buffer, 0, size);
            return total;
        }

        public static void Close()
        {
            mmva?.Dispose();
            mmf?.Dispose();
        }

        public static void Clear()
        {
            var buffer = new int[2] { 0, 0 };
            mmva?.WriteArray<int>(0, buffer, 0, 2);
        }
    }
}
