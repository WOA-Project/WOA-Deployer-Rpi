using System.Threading.Tasks;
using Deployer.Raspberry.Tasks;
using Xunit;

namespace Deployer.Tests.Tasks
{
    public class DriversDownloadTests
    {
        [Fact]
        public async Task Test()
        {
            var task = new DriversDownload(new GitHubDownloader(), new FileSystemOperations());
            await task.Execute();
        }
    }
}