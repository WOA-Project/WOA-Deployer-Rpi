using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Deployer
{
    public class GitHubDownloader : IGitHubDownloader
    {
        public async Task<Stream> OpenZipStream(string mainUrl)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var matches = Regex.Match(mainUrl, "https://github\\.com/([\\w-]*)/([\\w-]*)");
                var username = matches.Groups[1].Value;
                var repository = matches.Groups[2].Value;
                var branch = "master";

                var url = $"https://github.com/{username}/{repository}/archive/{branch}.zip";

                var openZipStream = await client.GetStreamAsync(url);
                return openZipStream;
            }
        }
    }
}