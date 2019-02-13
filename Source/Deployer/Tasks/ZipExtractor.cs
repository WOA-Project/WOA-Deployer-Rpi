using System.IO;
using System.IO.Compression;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.Utils;

namespace Deployer.Tasks
{
    public class ZipExtractor : IZipExtractor
    {
        private readonly IFileSystemOperations fileSystemOperations;

        public ZipExtractor(IFileSystemOperations fileSystemOperations)
        {
            this.fileSystemOperations = fileSystemOperations;
        }

        public async Task ExtractFirstChildToFolder(Stream stream, string folderPath)
        {
            await Extract(stream, folderPath, FileUtils.GetTempDirectoryName());
            await MoveFirstChildToDestination(folderPath, FileUtils.GetTempDirectoryName());
        }

        public async Task ExtractToFolder(Stream stream, string folderPath)
        {
            var tempDir = FileUtils.GetTempDirectoryName();
            await Extract(stream, folderPath, tempDir);
            await MoveToDestination(tempDir, folderPath);
        }

        private async Task MoveToDestination(string source, string destination)
        {
            await fileSystemOperations.CopyDirectory(source, destination);
            await fileSystemOperations.DeleteDirectory(source);
        }

        private async Task MoveFirstChildToDestination(string source, string destination)
        {
            var folderName = Path.GetFileName(destination);
            var firstChild = Path.Combine(source, folderName);

            await fileSystemOperations.CopyDirectory(firstChild, destination);
            await fileSystemOperations.DeleteDirectory(source);
        }

        private async Task Extract(Stream stream, string folderPath, string temp)
        {
            await Observable.Start(() =>
            {
                using (var zip = new ZipArchive(stream, ZipArchiveMode.Read, false))
                {
                    zip.ExtractToDirectory(temp);
                }
            });

            if (fileSystemOperations.DirectoryExists(folderPath))
            {
                await fileSystemOperations.DeleteDirectory(folderPath);
            }
        }
    }
}