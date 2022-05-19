using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC
{
    public class BinarySerializer
    {


        public static T Clone<T>(T obj) where T : class
        {
            var data = Serialize(obj);
            return Deserialize<T>(data);
        }

        public static byte[] Serialize(object obj)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                var ms = new MemoryStream();
                formatter.Serialize(ms, obj);
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static T Deserialize<T>(byte[] data) where T : class
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                var ms = new MemoryStream(data);
                var obj = formatter.Deserialize(ms);
                return obj as T;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
