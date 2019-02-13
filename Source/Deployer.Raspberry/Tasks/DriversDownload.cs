using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Raspberry.Tasks
{
    [TaskDescription("Downloading Main Driver Package")]
    public class DriversDownload : IDeploymentTask
    {
        private readonly IGitHubDownloader downloader;
        private readonly IFileSystemOperations fileSystemOperations;
        private const string DownloadFolder = @"Downloaded\Drivers";

        public DriversDownload(IGitHubDownloader downloader, IFileSystemOperations fileSystemOperations)
        {
            this.downloader = downloader;
            this.fileSystemOperations = fileSystemOperations;
        }

        public async Task Execute()
        {
            if (fileSystemOperations.DirectoryExists("Drivers"))
            {
                return;
            }

            using (var stream = await downloader.OpenZipStream("https://github.com/andreiw/RaspberryPiPkg"))
            {
                var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);

                var root = zipArchive.Entries.First(x => x.FullName.EndsWith("Drivers/"));

                var contents = zipArchive.Entries.Where(x => x.FullName.StartsWith(root.FullName) && !x.FullName.EndsWith("/"));
                
                await ExtractContents(DownloadFolder, root, contents);
            }
        }

        private async Task ExtractContents(string destination, ZipArchiveEntry baseEntry,
            IEnumerable<ZipArchiveEntry> entries)
        {
            foreach (var entry in entries)
            {
                var filePath = entry.FullName.Substring(baseEntry.FullName.Length);

                var destFile = Path.Combine(destination, filePath.Replace("/", "\\"));
                var dir = Path.GetDirectoryName(destFile);
                if (!fileSystemOperations.DirectoryExists(dir))
                {
                    fileSystemOperations.CreateDirectory(dir);
                }

                using (var destStream = File.Open(destFile, FileMode.OpenOrCreate))
                using (var stream = entry.Open())
                {
                    await stream.CopyToAsync(destStream);
                }
            }
        }
    }
}