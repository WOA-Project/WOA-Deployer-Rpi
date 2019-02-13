using System.IO;
using System.Threading.Tasks;
using Serilog;

namespace Deployer.Execution.Testing
{
    public class TestFileSystemOperations : IFileSystemOperations
    {
        public Task Copy(string source, string destination)
        {
            Log.Verbose("Copied {Source} to {Destination}", source, destination);
            return Task.CompletedTask;
        }

        public Task CopyDirectory(string source, string destination)
        {
            Log.Verbose("Copied folder {Source} to {Destination}", source, destination);
            return Task.CompletedTask;
        }

        public Task DeleteDirectory(string path)
        {
            Log.Verbose("Delete folder {Folder}", path);
            return Task.CompletedTask;
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
        }
    }
}