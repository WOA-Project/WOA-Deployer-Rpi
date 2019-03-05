using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.Execution;
using Serilog;

namespace Deployer.Raspberry.Tasks
{
    [TaskDescription("Downloading UEFI")]
    public class DownloadUefi : IDeploymentTask
    {
        private readonly string destination;
        private readonly IFileSystemOperations fileSystemOperations;
        
        public DownloadUefi(string destination, IFileSystemOperations fileSystemOperations)
        {
            this.destination = destination;
            this.fileSystemOperations = fileSystemOperations;
        }

        public async Task Execute()
        {
            if (fileSystemOperations.DirectoryExists(destination))
            {
                Log.Warning("UEFI was already downloaded. Skipping download.");
                return;
            }

            using (var stream = await GitHubMixin.OpenBranchStream("https://github.com/andreiw/RaspberryPiPkg"))
            {
                var zipArchive = await Observable.Start(() => new ZipArchive(stream, ZipArchiveMode.Read));
                var mostRecentFolderEntry = GetMostRecentDirEntry(zipArchive);
                var contents = zipArchive.Entries.Where(x => x.FullName.StartsWith(mostRecentFolderEntry.FullName) && !x.FullName.EndsWith("/"));
                await ExtractContents(mostRecentFolderEntry, contents);
            }
        }

        private async Task ExtractContents(ZipArchiveEntry baseEntry,
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

        private ZipArchiveEntry GetMostRecentDirEntry(ZipArchive p)
        {
            var dirs = from e in p.Entries
                where e.FullName.EndsWith("/")
                select e;

            var splitted = from e in dirs
                select new
                {
                    e,
                    Parts = e.FullName.Split('/'),
                };

            var parsed = from r in splitted
                select new
                {
                    r.e,
                    Date = FirstParseableOrNull(r.Parts),
                };

            return parsed.OrderByDescending(x => x.Date).First().e;
        }

        private DateTime? FirstParseableOrNull(string[] parts)
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