using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.Common
{
    public class BinarySerializer
    {

        sealed class SearchAssembliesBinder : SerializationBinder
        {
            private readonly bool _searchInDlls;
            private readonly Assembly _currentAssembly;

            public SearchAssembliesBinder(Assembly currentAssembly, bool searchInDlls)
            {
                _currentAssembly = currentAssembly;
                _searchInDlls = searchInDlls;
            }

            public override Type BindToType(string assemblyName, string typeName)
            {
                List<AssemblyName> assemblyNames = new List<AssemblyName>();
                assemblyNames.Add(_currentAssembly.GetName()); // EXE

                if (_searchInDlls)
                {
                    assemblyNames.AddRange(_currentAssembly.GetReferencedAssemblies()); // DLLs
                }

                foreach (AssemblyName an in assemblyNames)
                {
                    var typeToDeserialize = GetTypeToDeserialize(typeName, an);
                    if (typeToDeserialize != null)
                    {
                        return typeToDeserialize; // found
                    }
                }

                return null; // not found
            }

            private static Type GetTypeToDeserialize(string typeName, AssemblyName an)
            {
                string fullTypeName = string.Format("{0}, {1}", typeName, an.FullName);
                var typeToDeserialize = Type.GetType(fullTypeName);
                return typeToDeserialize;
            }




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
                formatter.Binder = new SearchAssembliesBinder(Assembly.GetExecutingAssembly(), true);
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
