using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Deployer.Utils
{
    public static class FileUtils
    {
        public static bool EnsureExistingPaths(this string[] pathsToCheck)
        {
            return pathsToCheck.All(IsExistingPath);
        }

        public static async Task Copy(string source, string destination, CancellationToken cancellationToken)
        {
            var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            var bufferSize = 4096;

            using (var sourceStream =
                new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions))

            using (var destinationStream =
                new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, fileOptions))

                await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken).ConfigureAwait(false);
        }

        public static async Task Copy(string source, string destination, FileMode fileMode = FileMode.Create)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (destination.EndsWith(char.ToString(Path.DirectorySeparatorChar)))
            {
                destination = Path.Combine(destination, Path.GetFileName(source));
            }

            var dir = Path.GetDirectoryName(destination);

            if (dir == null)
            {
                throw new InvalidOperationException("The directory name could be retrieved");
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            var bufferSize = 4096;

            using (var sourceStream =
                new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions))

            using (var destinationStream =
                new FileStream(destination, fileMode, FileAccess.Write, FileShare.None, bufferSize, fileOptions))

                await sourceStream.CopyToAsync(destinationStream, bufferSize).ConfigureAwait(false);
        }

        public static Task CopyDirectory(string source, string destination)
        {
            return CopyDirectory(new DirectoryInfo(source), new DirectoryInfo(destination));
        }

        private static async Task CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            foreach (var dir in source.GetDirectories())
            {
                await CopyDirectory(dir, destination.CreateSubdirectory(dir.Name));
            }

            foreach (var file in source.GetFiles())
            {
                var destFileName = Path.Combine(destination.FullName, file.Name);
                await Copy(file.FullName, destFileName);
            }
        }

        private static bool IsExistingPath(string path)
        {
            var isExistingPath = File.Exists(path) || Directory.Exists(path);
            var status = isExistingPath ? "exists" : "does not exist";
           
            Log.Verbose($"The '{{Path}}' {status}", path);

            return isExistingPath;
        }

        public static void CreateDirectory(string destPath)
        {
            if (!IsExistingPath(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
        }

        public static string GetTempDirectoryName() 
        {
            var randomFilename = Path.GetRandomFileName();
            return Path.Combine(Path.GetTempPath(), randomFilename);
        }
    }
}