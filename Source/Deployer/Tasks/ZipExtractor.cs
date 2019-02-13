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

        public async Task ExtractToFolder(Stream stream, string folderPath)
        {                  
            var temp = FileUtils.GetTempDirectoryName();

            await Observable.Start(() =>
            {
                using (var zip = new ZipArchive(stream, ZipArchiveMode.Read, false))
                {
                    zip.ExtractToDirectory(temp);
                }
            });
            
            var folderName = Path.GetFileName(folderPath);

            if (fileSystemOperations.DirectoryExists(folderPath))
            {
                await fileSystemOperations.DeleteDirectory(folderPath);
            }

            var firstChild = Path.Combine(temp, folderName);
            
            await fileSystemOperations.CopyDirectory(firstChild, folderPath);
            await fileSystemOperations.DeleteDirectory(temp);
        }
    }
}