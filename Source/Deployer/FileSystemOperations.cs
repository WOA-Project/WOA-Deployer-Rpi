using System.IO;
using System.Threading.Tasks;
using Deployer.Utils;
using Serilog;

namespace Deployer
{
    public class FileSystemOperations : IFileSystemOperations
    {
        public Task Copy(string source, string destination)
        {
            Log.Debug("Copying file {Source} to {Destination}", source, destination);

            return FileUtils.Copy(source, destination);
        }

        public Task CopyDirectory(string source, string destination)
        {
            Log.Verbose("Copying directory {Source} to {Destination}", source, destination);
            return FileUtils.CopyDirectory(source, destination);
        }

        public Task DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
            return Task.CompletedTask;
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}