using Deployer.Execution;
using Deployer.Tasks;
using Serilog;
using SharpCompress.Archives.Zip;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Deployer.Raspberry.Tasks
{
    [TaskDescription("Downloading UEFI")]
    public class DownloadUefi : IDeploymentTask
    {
        private readonly string destination;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IZipExtractor extractor;
        private readonly IObserver<double> progressObserver;

        public DownloadUefi(string destination, IFileSystemOperations fileSystemOperations, IZipExtractor extractor, IObserver<double> progressObserver)
        {
            this.destination = destination;
            this.fileSystemOperations = fileSystemOperations;
            this.extractor = extractor;
            this.progressObserver = progressObserver;
        }

        public async Task Execute()
        {
            if (fileSystemOperations.DirectoryExists(destination))
            {
                Log.Warning("UEFI was already downloaded. Skipping download.");
                return;
            }

            using (var stream = await GitHubMixin.GetBranchZippedStream("https://github.com/andreiw/RaspberryPiPkg", progressObserver: progressObserver))
            {
                await extractor.ExtractRelativeFolder(stream, GetMostRecentDirEntry, destination, progressObserver);
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