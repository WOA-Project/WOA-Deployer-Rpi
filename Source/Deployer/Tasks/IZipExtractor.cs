using System.IO;
using System.Threading.Tasks;

namespace Deployer.Tasks
{
    public interface IZipExtractor
    {
        Task ExtractToFolder(Stream stream, string folder);
    }
}