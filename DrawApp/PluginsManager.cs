using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DrawApp
{
    static public class PluginsManager
    {
        static public IEnumerable<T> LoadPlugins<T>(string pluginsDirectory = @"C:\PROJEKTY\drawapp\AppPlugins\")
        {
            var plugins = new List<T>();
            var assemblies = LoadAssemblies(pluginsDirectory);
            foreach (var a in assemblies)
            {
                T plugin = default(T);

                try
                {
                    plugin = CreatePluginsFromAssemblies<T>(a);
                    plugins.Add(plugin);
                }
                catch (Exception)
                {
                }
            }

            return plugins;
        }

        static private T CreatePluginsFromAssemblies<T>(Assembly assembly)
        {
            var selectedType = assembly.GetTypes()
                .Where(t => t.IsPublic
                    && t.IsClass
                    && typeof(T).IsAssignableFrom(t))
                .FirstOrDefault();

            if (selectedType == null)
            {
                throw new Exception();
            }

            var instance = (T)Activator.CreateInstance(selectedType);
            return instance;
        }

        static private IEnumerable<Assembly> LoadAssemblies(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath)
                .Where(f => f.EndsWith(".dll"))
                .Select(f => Assembly.LoadFile(f));

            return files;
        }
    }
}
