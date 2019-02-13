using System.Threading.Tasks;
using Deployer.Tasks;
using Xunit;

namespace Deployer.Tests.Tasks
{
    public class GitHubUnpackTests
    {
        [Fact(Skip = "Long running")]
        public async Task Test()
        {
            var task = new GitHubUnpack("https://github.com/gus33000/MSM8994-8992-NT-ARM64-Drivers",
                new GitHubDownloader(), new ZipExtractor(new FileSystemOperations()));
            await task.Execute();
        }
    }    
}