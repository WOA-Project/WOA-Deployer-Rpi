using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Deployer.Execution;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Fetching from GitHub: {0}")]
    public class GitHubUnpack : IDeploymentTask
    {
        private readonly string downloadUrl;
        private readonly IGitHubDownloader downloader;
        private readonly IZipExtractor extractor;
        private string repository;
        private string branch;
        private string folderName;
        private string folderPath;
        private const string SubFolder = "Downloaded";

        public GitHubUnpack(string downloadUrl, IGitHubDownloader downloader, IZipExtractor extractor)
        {
            ParseUrl(downloadUrl);
            this.downloadUrl = downloadUrl;
            this.downloader = downloader;
            this.extractor = extractor;
        }

        private void ParseUrl(string url)
        {
            var matches = Regex.Match(url, "https://github\\.com/([\\w-]*)/([\\w-]*)");
            repository = matches.Groups[2].Value;
            branch = "master";
            folderName = repository + "-" + branch;
            folderPath = Path.Combine(SubFolder, folderName);
        }

        public async Task Execute()
        {
            if (Directory.Exists(folderPath))
            {
                Log.Warning("{Pack} was already downloaded. Skipping download.", repository);
                return;
            }

            var openZipStream = await downloader.OpenZipStream(downloadUrl);
            using (var stream = openZipStream)
            {
                await extractor.ExtractFirstChildToFolder(stream, folderPath);
            }
        }
    }
}