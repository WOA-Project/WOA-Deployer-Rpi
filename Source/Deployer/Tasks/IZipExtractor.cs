using System.IO;
using System.Threading.Tasks;

namespace Deployer.Tasks
{
    public interface IZipExtractor
    {
        Task ExtractFirstChildToFolder(Stream stream, string folder);
        Task ExtractToFolder(Stream stream, string folderPath);
    }
}