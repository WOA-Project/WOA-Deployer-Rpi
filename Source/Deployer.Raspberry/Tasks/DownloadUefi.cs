using Deployer.Execution;
using Deployer.Tasks;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ZipArchiveEntry = SharpCompress.Archives.Zip.ZipArchiveEntry;

namespace Deployer.Raspberry.Tasks
{
    [TaskDescription("Downloading UEFI")]
    public class DownloadUefi : IDeploymentTask
    {
        private readonly string destination;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IZipExtractor extractor;
        private readonly IOperationProgress progress;
        private readonly IDownloader downloader;

        public DownloadUefi(string destination, IFileSystemOperations fileSystemOperations, IZipExtractor extractor, IOperationProgress progress, IDownloader downloader)
        {
            this.destination = destination;
            this.fileSystemOperations = fileSystemOperations;
            this.extractor = extractor;
            this.progress = progress;
            this.downloader = downloader;
        }

        public async Task Execute()
        {
            if (fileSystemOperations.DirectoryExists(destination))
            {
                Log.Warning("UEFI was already downloaded. Skipping download.");
                return;
            }

            using (var stream = await GitHubMixin.GetBranchZippedStream(downloader,
                "https://github.com/andreiw/RaspberryPiPkg", progressObserver: progress))
            {
                await extractor.ExtractRelativeFolder(stream, GetMostRecentDirEntry, destination, progress);
            }
        }

        private static ZipArchiveEntry GetMostRecentDirEntry(IEnumerable<ZipArchiveEntry> entries)
        {
            var split = from dir in entries
                where dir.IsDirectory
                select new
            {
                Directory = dir,
                Parts = dir.Key.Split('/'),
            };

            var parsed = from r in split
                select new
                {
                    r.Directory,
                    Date = FirstParseableOrNull(r.Parts),
                };

            return parsed.OrderByDescending(x => x.Date).First().Directory;
        }

        private static DateTime? FirstParseableOrNull(IEnumerable<string> parts)
        {
            foreach (var part in parts)
            {
                var candidate = part.Split('-');
                if (candidate.Length > 1)
                {
                    var datePart = candidate[0];
                    if (DateTime.TryParseExact(datePart, "yyyyMMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    {
                        return date;
                    }
                }
            }

            return null;
        }
    }
}