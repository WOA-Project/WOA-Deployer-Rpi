using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Deployer.Filesystem.FullFx
{
    public static class Assemblies
    {
        public static IEnumerable<Assembly> ReferencedAssemblies => Assembly.GetCallingAssembly().GetReferencedAssemblies().Select(Assembly.Load);
        public static IEnumerable<Assembly> AppDomainAssemblies => AppDomain.CurrentDomain.GetAssemblies();
        public static IEnumerable<Assembly> AssembliesInAppFolder
        {
            get
            {
                var entryAssembly = Assembly.GetExecutingAssembly();
                var path = entryAssembly.Location;
                var folder = Path.GetDirectoryName(path);
                var assemblies = new Collection<Assembly>();

                var fileNames = FilterFiles(folder, ".dll", ".exe");

                foreach (var fileName in fileNames)
                {
                    try
                    {
                        assemblies.Add(Assembly.LoadFile(fileName));
                    }
                    catch (FileNotFoundException)
                    {
                    }
                    catch (FileLoadException)
                    {
                    }
                    catch (BadImageFormatException)
                    {
                    }
                }

                return assemblies;
            }
        }

        public static IEnumerable<string> FilterFiles(string path, params string[] extensionsWithNoWildcard)
        {
            return
                Directory
                    .EnumerateFiles(path, "*.*")
                    .Where(file => extensionsWithNoWildcard.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase)));
        }
    }
}