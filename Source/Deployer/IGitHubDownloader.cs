using System.IO;
using System.Threading.Tasks;

namespace Deployer
{
    public interface IGitHubDownloader
    {
        Task<Stream> OpenZipStream(string url);
    }
}