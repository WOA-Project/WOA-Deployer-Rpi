using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    public class ZipUnpack : IDeploymentTask
    {
        private readonly string url;
        private readonly string path;
        private readonly IZipExtractor extractor;
        private readonly IFileSystemOperations fileSystemOperations;

        public ZipUnpack(string url, string path, IZipExtractor extractor, IFileSystemOperations fileSystemOperations)
        {
            this.url = url;
            this.path = path;
            this.extractor = extractor;
            this.fileSystemOperations = fileSystemOperations;
        }

        public async Task Execute()
        {
            var folderPath = Path.Combine("Downloaded", path);

            if (fileSystemOperations.DirectoryExists(folderPath))
            {
                return;
            }

            using (var httpClient = new HttpClient())
            {
                var stream = await httpClient.GetStreamAsync(url);
                
                await extractor.ExtractToFolder(stream, folderPath);
            }
        }
    }
}