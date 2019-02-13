using System.Threading.Tasks;
using Deployer.Raspberry.Tasks;
using Xunit;

namespace Deployer.Tests.Tasks
{
    public class UefiDownloadTests
    {
        [Fact]
        public async Task Test()
        {
            var task = new UefiDownload(new GitHubDownloader(), new FileSystemOperations());
            await task.Execute();
        }
    }
}